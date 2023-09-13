using System.Globalization;
using CommandLine;
using CompaniesHouse.Response.Officers;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wealtherty.Cli.CompaniesHouse;
using Wealtherty.Cli.CompaniesHouse.Graph.Model;
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
        
        var things = new List<Thing>();

        foreach (var thinkTank in thinkTanks)
        {
            Log.Debug("ThinkTank: {@ThinkTank}", thinkTank);
            
            var thinkTankCompanies = companies
                .Where(x => x.OttId == thinkTank.OttId && x.CompanyNumber != null)
                .OrEmpty()
                .ToArray();

            foreach (var thinkTankCompany in thinkTankCompanies)
            {   
                Log.Debug("ThinkTankCompany: {@ThinkTankCompany}", thinkTankCompany);
                
                var company = await companiesHouseClient.GetCompanyProfileAsync(thinkTankCompany.CompanyNumber);
                if (company.Data == null)
                {
                    Log.Warning("ThinkTank Company doesn't exist - CompanyNumber: {CompanyNumber}", thinkTankCompany.CompanyNumber);
                    continue;
                }
                if (company.Data.SicCodes == null)
                {
                    Log.Warning("ThinkTank Company doesn't have any SIC Codes - Company: {@Company}", company.Data);
                    continue;
                }

                var officers = await companiesHouseClient.GetOfficersAsync(thinkTankCompany.CompanyNumber);

                foreach (var officer in officers.Take(2))
                {
                    Log.Debug("Officer: {@Officer}", officer);

                    
                    if (RolesToIgnore.Contains(officer.OfficerRole))
                    {
                        Log.Warning("Ignoring - Officer: {@Officer}", officer);
                        continue;
                    }

                    var appointments = await companiesHouseClient.GetAppointmentsAsync(officer.Links.Officer.OfficerId);

                    foreach (var appointment in appointments)
                    {
                        Log.Debug("Appointment: {@Appointment}", appointment);
                        
                        var appointmentCompany =
                            await companiesHouseClient.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber);
                        
                        if (appointmentCompany.Data == null)
                        {
                            Log.Warning("Appointment Company doesn't exist - CompanyNumber: {CompanyNumber}", appointment.Appointed.CompanyNumber);
                            continue;
                        }
                        if (appointmentCompany.Data.SicCodes == null)
                        {
                            Log.Warning("Appointment Company doesn't have any SIC Codes - Company: {@Company}", appointmentCompany.Data);
                            continue;
                        }

                        foreach (var code in appointmentCompany.Data.SicCodes)
                        {
                            var sicCode = sicCodeReader.Read(code);
                            
                            Log.Debug("SicCode: {@SicCode}", sicCode);
                            
                            var thing = new Thing
                            {
                                ThinkTankPoliticalWing = thinkTank.PoliticalWing,
                                ThinkTankName = thinkTank.Name,
                                ThinkTankFoundedOn = thinkTank.FoundedOn,
                        
                                CompanyNumber = appointmentCompany.Data.CompanyNumber,
                                CompanyName = appointmentCompany.Data.GetFormattedName(),
                                CompanyDateOfCreation = appointmentCompany.Data.DateOfCreation,
                                CompanyDateOfCessation = appointmentCompany.Data.DateOfCessation,
                                CompanySicCode = code,
                                CompanySicCodeCategory = sicCode?.Category,
                                CompanySicCodeDescription = sicCode?.Description,
                        
                                OfficerId = officer.Links.Officer.OfficerId,
                                OfficerName = officer.GetFormattedName(),
                                OfficerRole = appointment.OfficerRole.ToString(),
                                OfficerAppointedOn = appointment.AppointedOn,
                                OfficerResignedOn = appointment.ResignedOn
                            };
                    
                            things.Add(thing);
                        }
                    }
                    
                }
            }
        }

        await WriteToCsvFileAsync(things, "all_appointments.csv");

        var years = new[] { 2000, 2005, 2010, 2015 };

        var dates = years.Select(year => new KeyValuePair<int, Tuple<DateTime, DateTime>>(year,
            Tuple.Create(new DateTime(year, 1, 1),
                new DateTime(year, 1, 1).AddYears(5).AddMilliseconds(-1))))
            .ToDictionary(x => x.Key, x=> x.Value);

        foreach (var date in dates)
        {
            var thingsForDate = things
                .Where(x => x.ThinkTankFoundedOn <= date.Value.Item2)
                .Where(x => x.OfficerAppointedOn <= date.Value.Item2)
                .ToArray();
            
            await WriteToCsvFileAsync(thingsForDate, $"appointments_for_{date.Key}.csv");

            var politicalSicCodes = thingsForDate.GroupBy(x => new
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
            
            await WriteToCsvFileAsync(politicalSicCodes, $"sic_codes_for_{date.Key}.csv");

            var politicalSicCodeCategories = thingsForDate.GroupBy(x => new
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
            
            await WriteToCsvFileAsync(politicalSicCodeCategories, $"sic_code_categories_for_{date.Key}.csv");
        }
    }

    private static async Task WriteToCsvFileAsync<T>(IEnumerable<T> things, string path)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(things);
    }

    private static IEnumerable<T> ReadCsvFile<T>(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var rows =
            csv.GetRecords<T>()
                .ToArray();
        
        return rows;

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

    public class Thing
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