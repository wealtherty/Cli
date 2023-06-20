using AutoMapper;
using CommandLine;
using CompaniesHouse;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("ch:company-get")]
public class GetCompany : Command
{
    [Option('n', "number", Required = true)]
    public string Number { get; set; }

    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var companiesHouseClient = serviceProvider.GetService<ICompaniesHouseClient>();
        var client = serviceProvider.GetService<Client>();
        var mapper = serviceProvider.GetService<IMapper>();
        var driver = serviceProvider.GetService<IDriver>();

        await using var session = driver.AsyncSession();

        var officers = await client.GetOfficersAsync(Number);

        foreach (var officer in officers)
        {
            var officerNode = mapper.Map<Officer>(officer);

            var appointments = await client.GetAppointmentsAsync(officer.Links.Officer.OfficerId);
            
            foreach (var appointment in appointments)
            {
                var getAppointmentCompanyResponse =
                    await companiesHouseClient.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber);
                var appointmentCompanyNode = mapper.Map<Company>(getAppointmentCompanyResponse.Data);

                officerNode.AddRelation(new Appointment(officerNode, appointmentCompanyNode, appointment));
            }

            await session.ExecuteCommandsAsync(officerNode);
        }
    }
}