using Neo4j.Driver;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Services;

public class Facade
{
    private readonly IDriver _driver;
    private readonly Client _client;

    public Facade(IDriver driver, Client client)
    {
        _driver = driver;
        _client = client;
    }
    
    public async Task ModelCompanyAsync(string companyNumber, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();

        var officers = await _client.GetOfficersAsync(companyNumber, cancellationToken);

        foreach (var officer in officers)
        {
            var officerNode = new Officer(officer);

            var appointments = await _client.GetAppointmentsAsync(officer.Links.Officer.OfficerId, cancellationToken);
            
            foreach (var appointment in appointments)
            {
                var getAppointmentCompanyResponse = await _client.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber, cancellationToken);
                var appointmentCompanyNode = new Company(getAppointmentCompanyResponse.Data);

                officerNode.AddRelation(new Appointment(officerNode, appointmentCompanyNode, appointment));
            }

            await session.ExecuteCommandsAsync(officerNode);
        }

    }
}