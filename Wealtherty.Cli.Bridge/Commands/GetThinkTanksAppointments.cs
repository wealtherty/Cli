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
        var officerIds = new HashSet<string>();

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
            
            Log.Debug("Got Company Officers - Company: {CompanyNumber}, Officers: {@Officers}", company.CompanyNumber, officers);

            var officersOfInterest = officers
                .Where(x => !RolesToIgnore.Contains(x.OfficerRole))
                .Select(x => new
                {
                    Id = x.Links.Officer.OfficerId,
                    Name = x.GetFormattedName()
                })
                .Distinct()
                .ToArray();
            
            Log.Debug("Filtered Company Officers - Officers-of-interest: {OfficersOfInterest}", officersOfInterest);

            foreach (var officer in officersOfInterest)
            {
                if (officerIds.Contains(officer.Id))
                {
                    Log.Debug("Ignoring duplicate Officer - Id: {OfficerId}", officer.Id);
                    continue;
                }
                
                var officerAppointments = await companiesHouseClient.GetAppointmentsAsync(officer.Id);
                Log.Debug("Got Officer Appointments - OfficerId: {OfficerId}, Appointments: {@Appointments}", officer.Id, officerAppointments);

                officerIds.Add(officer.Id);
                
                foreach (var officerAppointment in officerAppointments)
                {
                    if (RolesToIgnore.Contains(officerAppointment.OfficerRole))
                    {
                        Log.Warning(
                            "Ignoring Appointment: Officer-Role is excluded - PoliticalWing: {PoliticalWing}, ThinkTank: {ThinkTank}, CompanyNumber: {CompanyNumber}, OfficerName: {OfficerName}, OfficerRole: {OfficerRole}",
                            thinkTank.PoliticalWing, thinkTank.Name, company.CompanyNumber,
                            officer.Name, officerAppointment.OfficerRole);
                        continue;
                    }

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

                            OfficerId = officer.Id,
                            OfficerName = officer.Name,
                            OfficerRole = officerAppointment.OfficerRole.ToString(),

                            OfficerAppointedOn = officerAppointment.AppointedOn,
                            OfficerResignedOn = officerAppointment.ResignedOn,
                        };
                        
                        var appointedOnKey = appointment.OfficerAppointedOn.HasValue ? appointment.OfficerAppointedOn.Value.ToString("d") : "Unknown";
                        var resignedOnKey = appointment.OfficerResignedOn.HasValue ? appointment.OfficerResignedOn.Value.ToString("d") : "Unknown";
                        var key = $"{appointment.ThinkTankId}_{appointment.CompanyNumber}_{appointment.CompanySicCode}_{appointment.OfficerId}_{appointedOnKey}_{appointment.OfficerRole}_{resignedOnKey}";

                        if (appointments.ContainsKey(key))
                        {
                            Log.Warning("Ignoring duplicate appointment - Key: {@Key}", key);
                            continue;
                        }
                        
                        Log.Information("Added appointment - Key: {@Key}", key);
                        appointments.Add(key, appointment);
                    }
                }
            }
        }

        var rows = appointments
            .Values
            .OrderBy(x => x.OfficerAppointedOn)
            .OrderBy(x => x.OfficerName)
            .OrderBy(x => x.CompanySicCode)
            .ToArray();
        
        await outputWriter.WriteToCsvFileAsync(rows, "..\\Wealtherty.ThinkTanks\\Resources\\Appointments.csv", useOutputDirectory: false);
    }
}