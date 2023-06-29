using CompaniesHouse.Response.Officers;
using Neo4j.Driver;
using Serilog;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;
using Officer = Wealtherty.Cli.CompaniesHouse.Model.Officer;

namespace Wealtherty.Cli.CompaniesHouse;

public class Facade
{
    private readonly IDriver _driver;
    private readonly Client _client;

    private static OfficerRole[] RolesToIgnore = {
        OfficerRole.CorporateNomineeSecretary,
        OfficerRole.CorporateSecretary,
        OfficerRole.NomineeDirector, 
        OfficerRole.NomineeSecretary
    };

    public Facade(IDriver driver, Client client)
    {
        _driver = driver;
        _client = client;
    }
    
    public async Task ModelCompanyAsync(string companyNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(companyNumber)) return;
        
        await using var session = _driver.AsyncSession();

        var officers = await _client.GetOfficersAsync(companyNumber, cancellationToken);

        foreach (var officer in officers)
        {
            if (RolesToIgnore.Contains(officer.OfficerRole))
            {
                Log.Information("Ignoring Officer: {@Officer}", new { Id = officer.Links.Officer.OfficerId, officer.Name, Role = officer.OfficerRole});
                continue;
            }
            
            var officerNode = new Officer(officer);

            var appointments = await _client.GetAppointmentsAsync(officer.Links.Officer.OfficerId, cancellationToken);
            
            foreach (var appointment in appointments.Where(x => !RolesToIgnore.Contains(x.OfficerRole)))
            {
                var getAppointmentCompanyResponse = await _client.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber, cancellationToken);
                var appointmentCompanyNode = new Company(getAppointmentCompanyResponse.Data);

                officerNode.AddRelation(new Appointment(officerNode, appointmentCompanyNode, appointment));
            }

            await session.ExecuteCommandsAsync(officerNode);
        }

    }
}