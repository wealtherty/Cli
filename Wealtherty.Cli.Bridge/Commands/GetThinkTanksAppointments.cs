using CommandLine;
using CompaniesHouse.Response.Officers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wealtherty.Cli.Bridge.Model.Csv;
using Wealtherty.Cli.CompaniesHouse;
using Wealtherty.Cli.Core;
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
        var thinkTanksReader = serviceProvider.GetRequiredService<Reader>();
        var outputWriter = serviceProvider.GetRequiredService<OutputWriter>();
        
        var thinkTanks = thinkTanksReader.GetThinkTanks();
        var companies = thinkTanksReader.GetThinkTanksCompanies()
            .Where(x => x.CompanyNumber != null)
            .ToArray();
        
        var allAppointments = new List<ThinkTankAppointment>();

        foreach (var thinkTank in thinkTanks)
        {
            Log.Debug("ThinkTank: {@ThinkTank}", thinkTank);
            
            var thinkTankCompanies = companies
                .Where(x => x.OttId == thinkTank.OttId)
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

        await outputWriter.WriteToCsvFileAsync(allAppointments, "all_appointments.csv");
    }
}