using CompaniesHouse.Response.Officers;
using Neo4j.Driver;
using Serilog;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.GraphDb;
using Officer = Wealtherty.Cli.CompaniesHouse.Model.Officer;

namespace Wealtherty.Cli.CompaniesHouse;

public class Facade
{
    private readonly IDriver _driver;
    private readonly Client _client;

    private static readonly OfficerRole[] RolesToIgnore = {
        OfficerRole.CorporateNomineeDirector,
        OfficerRole.CorporateNomineeSecretary,
        OfficerRole.CorporateSecretary,
        OfficerRole.NomineeDirector, 
        OfficerRole.NomineeSecretary
    };

    private static readonly string[] CompaniesToIgnore = Array.Empty<string>();

    private static readonly string[] OfficersToIgnore =
    {
        "SBjtBss_I4XEupbfAUXoeAkMcIk",
        "8d_bnTiwfxh8JIr3YfuwkmkWkCg"
    };

    public Facade(IDriver driver, Client client)
    {
        _driver = driver;
        _client = client;
    }
    
    public async Task ModelCompanyAsync(string companyNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(companyNumber)) return;
        
        if (CompaniesToIgnore.Contains(companyNumber, StringComparer.OrdinalIgnoreCase))
        {
            Log.Information("Ignoring Company: {Number}", companyNumber);
            return;
        }
        
        await using var session = _driver.AsyncSession();

        var officers = await _client.GetOfficersAsync(companyNumber, cancellationToken);

        foreach (var officer in officers)
        {
            if (OfficersToIgnore.Contains(officer.Links.Officer.OfficerId, StringComparer.OrdinalIgnoreCase) ||  RolesToIgnore.Contains(officer.OfficerRole))
            {
                Log.Information("Ignoring Officer: {@Officer}", new { Id = officer.Links.Officer.OfficerId, officer.Name, Role = officer.OfficerRole});
                continue;
            }
            
            var officerNode = new Officer(officer);

            var appointments = await _client.GetAppointmentsAsync(officer.Links.Officer.OfficerId, cancellationToken);
            
            foreach (var appointment in appointments.Where(x => !RolesToIgnore.Contains(x.OfficerRole)))
            {
                if (CompaniesToIgnore.Contains(appointment.Appointed.CompanyNumber, StringComparer.OrdinalIgnoreCase))
                {
                    Log.Information("Ignoring Appointment: {@Appointment}", new { OfficeId = officer.Links.Officer.OfficerId, officer.Name, appointment.Appointed.CompanyNumber, appointment.Appointed.CompanyName});
                    continue;
                }
                
                var getAppointmentCompanyResponse = await _client.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber, cancellationToken);
                var appointmentCompanyNode = new Company(getAppointmentCompanyResponse.Data);

                officerNode.AddRelation(new Appointment(officerNode, appointmentCompanyNode, appointment));
            }

            await session.ExecuteCommandsAsync(officerNode);
        }

    }

    public async Task ModelThinkTankAsync(string name, PoliticalWing politicalWing, string companyNumber, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        
        var thinkTank = new ThinkTank
        {
            Name = name,
            Wing = politicalWing.ToString()
        };
        
        thinkTank.AddRelation(new Relationship<ThinkTank, Company>(thinkTank, new Company(companyNumber), "HAS_COMPANY"));
        
        await session.ExecuteCommandsAsync(thinkTank);

        await ModelCompanyAsync(companyNumber, cancellationToken);
    }
}