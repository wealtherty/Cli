using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Wealtherty.Cli.CharityCommission.Graph.Model;
using Wealtherty.Cli.CompaniesHouse.Model.Graph;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.Bridge.Commands;

[Verb("br:connect-charities-to-companies")]
public class ConnectCharitiesAndCompanies : Command
{
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var session = serviceProvider.GetService<IAsyncSession>();
        var companiesHouseFacade = serviceProvider.GetService<CompaniesHouse.Facade>();
        var charityCommissionFacade = serviceProvider.GetService<CharityCommission.Facade>();

        var cancellationToken = new CancellationToken();

        var charityNodes = await charityCommissionFacade.GetCharitiesAsync();

        foreach (var charityNode in charityNodes)
        {
            if (charityNode.CompanyHouseNumber == null) continue;
            
            var companyNode = await companiesHouseFacade.GetCompanyAsync(charityNode.CompanyHouseNumber) ??
                              await companiesHouseFacade.CreateOfficersAndCompaniesAsync(charityNode.CompanyHouseNumber, cancellationToken);
            
            charityNode.AddRelation(new Relationship<Charity,Company>(charityNode, companyNode, "HAS_COMPANY"));
            
            await session.ExecuteCommandsAsync(charityNode);
        }
    }
}