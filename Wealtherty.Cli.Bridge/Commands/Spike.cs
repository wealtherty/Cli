using System.Globalization;
using CommandLine;
using CompaniesHouse.Response.Officers;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wealtherty.Cli.CompaniesHouse;
using Wealtherty.Cli.Core;
using Wealtherty.ThinkTanks.Csv.Model;
using Wealtherty.ThinkTanks.Graph.Model;
using ThinkTank = Wealtherty.ThinkTanks.Csv.Model.ThinkTank;

namespace Wealtherty.Cli.Bridge.Commands;

[Verb("spike")]
public class Spike : Command
{
    
    private static readonly OfficerRole[] RolesToIgnore = {
        OfficerRole.CorporateNomineeDirector,
        OfficerRole.CorporateNomineeSecretary,
        OfficerRole.CorporateSecretary,
        OfficerRole.NomineeDirector, 
        OfficerRole.NomineeSecretary
    };

    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var companiesHouseClient = serviceProvider.GetRequiredService<Client>();
        var sicCodeReader = serviceProvider.GetRequiredService<SicCodeReader>();
        
        var thinkTanks = ReadCsvFile<ThinkTank>("v2_thinktanks.csv");
        var companies = ReadCsvFile<ThinkTankCompany>("v2_thinktanks_companies.csv");
        
        var allAppointments = new List<ThinkTankAppointment>();

        foreach (var thinkTank in thinkTanks)
        {
            Log.Debug("ThinkTank: {@ThinkTank}", thinkTank);
            
            var thinkTankCompanies = companies
                .Where(x => x.OttId == thinkTank.OttId && x.CompanyNumber != null)
                .OrEmpty()
                .ToArray();

            foreach (var thinkTankCompany in thinkTankCompanies)
            {
                var company = await companiesHouseClient.GetCompanyProfileAsync(thinkTankCompany.CompanyNumber);
                if (company.Data == null)
                {
                    Log.Warning(
                        "Ignoring: ThinkTank-Company doesn't exist - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}",
                        thinkTank.PoliticalWing, thinkTank.Name, thinkTankCompany.CompanyNumber);
                    continue;
                }
                if (company.Data.SicCodes == null)
                {
                    Log.Warning(
                        "Ignoring: ThinkTank-Company doesn't have any SIC Codes - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}",
                        thinkTank.PoliticalWing, thinkTank.Name, company.Data.CompanyNumber);
                    continue;
                }

                var officers = await companiesHouseClient.GetOfficersAsync(thinkTankCompany.CompanyNumber);

                foreach (var officer in officers)
                {
                    if (RolesToIgnore.Contains(officer.OfficerRole))
                    {
                        Log.Warning(
                            "Ignoring: Officer-Role is excluded - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}, OfficerName: {OfficerName}, OfficerRole: {OfficerRole}",
                            thinkTank.PoliticalWing, thinkTank.Name, thinkTankCompany.CompanyNumber, 
                            officer.Name, officer.OfficerRole);
                        continue;
                    }

                    var appointments = await companiesHouseClient.GetAppointmentsAsync(officer.Links.Officer.OfficerId);

                    foreach (var appointment in appointments)
                    {
                        var appointmentCompany =
                            await companiesHouseClient.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber);
                        
                        if (appointmentCompany.Data == null)
                        {
                            Log.Warning(
                                "Ignoring: Company doesn't exist - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}",
                                thinkTank.PoliticalWing, thinkTank.Name, appointment.Appointed.CompanyNumber);
                            continue;
                        }
                        if (appointmentCompany.Data.SicCodes == null)
                        {
                            Log.Warning(
                                "Ignoring: Company doesn't have any SIC Codes - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}",
                                thinkTank.PoliticalWing, thinkTank.Name, appointmentCompany.Data.CompanyNumber);
                            continue;
                        }

                        foreach (var code in appointmentCompany.Data.SicCodes)
                        {
                            var sicCode = sicCodeReader.Read(code);
                            
                            if (sicCode == null)
                            {
                                Log.Warning(
                                    "Ignoring: SIC Code not recognised - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}, SicCode: {SicCode}",
                                    thinkTank.PoliticalWing, thinkTank.Name, appointmentCompany.Data.CompanyNumber, code);
                                continue;
                            }

                            var thinkTankAppointment = new ThinkTankAppointment
                            {
                                ThinkTankPoliticalWing = thinkTank.PoliticalWing,
                                ThinkTankName = thinkTank.Name,
                                ThinkTankFoundedOn = thinkTank.FoundedOn,
                        
                                CompanyNumber = appointmentCompany.Data.CompanyNumber,
                                CompanyName = appointmentCompany.Data.GetFormattedName(),
                                CompanyDateOfCreation = appointmentCompany.Data.DateOfCreation,
                                CompanyDateOfCessation = appointmentCompany.Data.DateOfCessation,
                                CompanySicCode = sicCode.Code,
                                CompanySicCodeCategory = sicCode.Category,
                                CompanySicCodeDescription = sicCode.Description,
                        
                                OfficerId = officer.Links.Officer.OfficerId,
                                OfficerName = officer.GetFormattedName(),
                                OfficerRole = appointment.OfficerRole.ToString(),
                                OfficerAppointedOn = appointment.AppointedOn,
                                OfficerResignedOn = appointment.ResignedOn
                            };
                    
                            allAppointments.Add(thinkTankAppointment);
                        }
                    }
                    
                }
            }
        }

        await WriteToCsvFileAsync(allAppointments, "all_appointments.csv");

        var years = new[] { 2000, 2005, 2010, 2015 };

        var dates = years.Select(year => new KeyValuePair<string, DateRange>($"{year}_to_{year + 5}",
                new DateRange
                {
                    FromDate = new DateTime(year, 1, 1),
                    ToDate = new DateTime(year, 1, 1).AddYears(5).AddMilliseconds(-1)
                }))
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var date in dates)
        {
            var appointmentsForDateRange = allAppointments
                .Where(x => x.ThinkTankFoundedOn <= date.Value.ToDate)
                .Where(x => x.CompanyDateOfCreation.HasValue && x.CompanyDateOfCreation <= date.Value.ToDate)
                .Where(x => !x.CompanyDateOfCessation.HasValue || (x.CompanyDateOfCessation.HasValue && x.CompanyDateOfCessation >= date.Value.FromDate))
                .Where(x => x.OfficerAppointedOn.HasValue && x.OfficerAppointedOn <= date.Value.ToDate)
                .Where(x => !x.OfficerResignedOn.HasValue || (x.OfficerResignedOn.HasValue && x.OfficerResignedOn >= date.Value.FromDate))
                .ToArray();
            
            await WriteToCsvFileAsync(appointmentsForDateRange, $"appointments_for_{date.Key}.csv");

            var sicCodesForDateRange = appointmentsForDateRange.GroupBy(x => new
                {
                    x.ThinkTankPoliticalWing,
                    x.CompanySicCode,
                    x.CompanySicCodeCategory,
                    x.CompanySicCodeDescription
                })
                .Select(x => new PoliticalSicCodeDescription
                {
                    PoliticalWing = x.Key.ThinkTankPoliticalWing,
                    SicCode = x.Key.CompanySicCode,
                    SicCodeCategory = x.Key.CompanySicCodeCategory,
                    SicCodeDescription = x.Key.CompanySicCodeDescription,
                    Count = x.ToList().Count
                })
                .OrderBy(x => x.PoliticalWing)
                .ThenByDescending(x => x.Count)
                .ToArray();
            
            await WriteToCsvFileAsync(sicCodesForDateRange, $"sic_codes_for_{date.Key}.csv");

            var sicCodeCategoriesForDateRange = appointmentsForDateRange.GroupBy(x => new
                {
                    x.ThinkTankPoliticalWing,
                    x.CompanySicCodeCategory
                })
                .Select(x => new PoliticalSicCodeCategory
                {
                    PoliticalWing = x.Key.ThinkTankPoliticalWing,
                    SicCodeCategory = x.Key.CompanySicCodeCategory,
                    Count = x.ToList().Count
                })
                .OrderBy(x => x.PoliticalWing)
                .ThenByDescending(x => x.Count)
                .ToArray();
            
            await WriteToCsvFileAsync(sicCodeCategoriesForDateRange, $"sic_code_categories_for_{date.Key}.csv");
        }
    }

    private static async Task WriteToCsvFileAsync<T>(IEnumerable<T> rows, string path)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(rows);
    }

    private static IEnumerable<T> ReadCsvFile<T>(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<T>()
            .ToArray();

    }

    public class DateRange
    {
        public DateTime FromDate { get; set; }
        
        public DateTime ToDate { get; set; }
    }

    public class PoliticalSicCodeCategory
    {
        public PoliticalWing PoliticalWing { get; set; }
        
        public string SicCodeCategory { get; set; }
        
        public int Count { get; set; }
    }

    public class PoliticalSicCodeDescription
    {
        public PoliticalWing PoliticalWing { get; set; }
        
        public string SicCode { get; set; }
        
        public string SicCodeCategory { get; set; }
        
        public string SicCodeDescription { get; set; }
        
        public int Count { get; set; }
        
    }

    public class ThinkTankAppointment
    {
        public PoliticalWing ThinkTankPoliticalWing { get; set; }
        
        public string ThinkTankName { get; set; }
        
        [Format("dd/MM/yyyy")]
        public DateTime ThinkTankFoundedOn { get; set; }
        
        
        public string OfficerId { get; set; }
        
        public string OfficerName { get; set; }
        
        public string OfficerRole { get; set; }
        
        [Format("dd/MM/yyyy")]
        public DateTime? OfficerAppointedOn { get; set; }
        
        [Format("dd/MM/yyyy")]
        public DateTime? OfficerResignedOn { get; set; }

        
        public string CompanyNumber { get; set; }
        
        public string CompanyName { get; set; }
        
        [Format("dd/MM/yyyy")]
        public DateTime? CompanyDateOfCreation { get; set; }
        
        [Format("dd/MM/yyyy")]
        public DateTime? CompanyDateOfCessation { get; set; }
        
        public string CompanySicCode { get; set; }
        
        
        public string CompanySicCodeCategory { get; set; }
        
        public string CompanySicCodeDescription { get; set; }
        
        
    }
}