using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("ch:companies-get")]
public class GetCompanies : Command
{
    private class CompanyNumbers
    {
        
        private const string AdamSmithInstitute = null;
        private const string BowGroup = null;
        private const string CentreForPolicyStudies = "01174651";
        private const string CentreForSocialJustice = "05137036";
        private const string InstitueOfEconomicAffairs = null;
        private const string PolicyExchange = null;
        private const string Reform = null;
        private const string LegatumInstitute = null;
        
        public static IEnumerable<string> All()
        {
            return new[]
            {
                CentreForSocialJustice,
            };
        }
    }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetService<Client>();
        var driver = serviceProvider.GetService<IDriver>();

        await using var session = driver.AsyncSession();

        foreach (var companyNumber in CompanyNumbers.All().Where(x => x != null))
        {
            var officers = await client.GetOfficersAsync(companyNumber);

            foreach (var officer in officers)
            {
                var officerNode = new Officer(officer);

                var appointments = await client.GetAppointmentsAsync(officer.Links.Officer.OfficerId);
            
                foreach (var appointment in appointments)
                {
                    var getAppointmentCompanyResponse = await client.GetCompanyProfileAsync(appointment.Appointed.CompanyNumber);
                    var appointmentCompanyNode = new Company(getAppointmentCompanyResponse.Data);

                    officerNode.AddRelation(new Appointment(officerNode, appointmentCompanyNode, appointment));
                }

                await session.ExecuteCommandsAsync(officerNode);
            }
            
        }

    }
}