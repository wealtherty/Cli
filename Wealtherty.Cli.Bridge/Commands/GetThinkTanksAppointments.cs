using CommandLine;
using CompaniesHouse.Response.Officers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wealtherty.Cli.CompaniesHouse;
using Wealtherty.Cli.Core;
using Wealtherty.ThinkTanks.Model.Csv;
using Wealtherty.ThinkTanks.Resources;

namespace Wealtherty.Cli.Bridge.Commands;

[Verb("br:get-thinktanks-appointments")]
public class GetThinkTanksAppointments : Command
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
        var thinkTanksReader = serviceProvider.GetRequiredService<ResourceReader>();
        var outputWriter = serviceProvider.GetRequiredService<OutputWriter>();
        
        var thinkTanks = thinkTanksReader.GetThinkTanks();
        var companies = thinkTanksReader.GetCompanies()
            .Where(x => x.CompanyNumber != null)
            .ToArray();

        var appointments = new Dictionary<string, Appointment>();

        foreach (var company in companies)
        {
            Log.Debug("Company: {@Company}", company);
            
            var thinkTank = thinkTanks.Single(x => x.OttId == company.OttId);
            Log.Debug("ThinkTank: {@ThinkTank}", thinkTank);

            var companyProfile = await companiesHouseClient.GetCompanyProfileAsync(company.CompanyNumber);
            if (companyProfile.Data == null)
            {
                Log.Warning(
                    "Ignoring: ThinkTank-Company doesn't exist - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}",
                    thinkTank.PoliticalWing, thinkTank.Name, company.CompanyNumber);
                continue;
            }

            if (companyProfile.Data.SicCodes == null)
            {
                Log.Warning(
                    "Ignoring: ThinkTank-Company doesn't have any SIC Codes - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}",
                    thinkTank.PoliticalWing, thinkTank.Name, companyProfile.Data.CompanyNumber);
                continue;
            }

            var officers = await companiesHouseClient.GetOfficersAsync(company.CompanyNumber);

            foreach (var officer in officers)
            {
                if (RolesToIgnore.Contains(officer.OfficerRole))
                {
                    Log.Warning(
                        "Ignoring: Officer-Role is excluded - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}, OfficerName: {OfficerName}, OfficerRole: {OfficerRole}",
                        thinkTank.PoliticalWing, thinkTank.Name, company.CompanyNumber,
                        officer.Name, officer.OfficerRole);
                    continue;
                }

                var officerAppointments = await companiesHouseClient.GetAppointmentsAsync(officer.Links.Officer.OfficerId);

                foreach (var officerAppointment in officerAppointments)
                {
                    var appointmentCompany =
                        await companiesHouseClient.GetCompanyProfileAsync(officerAppointment.Appointed.CompanyNumber);

                    if (appointmentCompany.Data == null)
                    {
                        Log.Warning(
                            "Ignoring: Company doesn't exist - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}",
                            thinkTank.PoliticalWing, thinkTank.Name, officerAppointment.Appointed.CompanyNumber);
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
                        
                        var appointment = new Appointment
                        {
                            ThinkTankPoliticalWing = thinkTank.PoliticalWing,
                            ThinkTankId = thinkTank.OttId,
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
                            OfficerRole = officerAppointment.OfficerRole.ToString(),

                            OfficerAppointedOn = officerAppointment.AppointedOn,
                            OfficerResignedOn = officerAppointment.ResignedOn,
                        };
                        
                        var appointedOnKey = appointment.OfficerAppointedOn.HasValue ? appointment.OfficerAppointedOn.Value.ToString("d") : "Unknown";
                        var resignedOnKey = appointment.OfficerResignedOn.HasValue ? appointment.OfficerResignedOn.Value.ToString("d") : "Unknown";
                        var key = $"{appointment.ThinkTankId}_{appointment.CompanyNumber}_{appointment.CompanySicCode}_{appointment.OfficerId}_{appointedOnKey}_{resignedOnKey}";

                        if (appointments.ContainsKey(key))
                        {
                            var origintal = appointments[key];
                            
                            Log.Warning("Ignoring: Duplicate - Original: {@Original}, Duplicate: {@Duplicate}", origintal, appointment);
                            continue;
                        }
                        
                        appointments.Add(key, appointment);
                    }
                }
            }
        }

        var rows = appointments
            .Values
            .OrderBy(x => x.OfficerName)
            .OrderBy(x => x.OfficerAppointedOn)
            .OrderBy(x => x.CompanySicCode)
            .ToArray();
        
        await outputWriter.WriteToCsvFileAsync(rows, "..\\Wealtherty.ThinkTanks\\Resources\\Appointments.csv", useOutputDirectory: false);
    }
}