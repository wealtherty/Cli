using Neo4j.Driver;
using Wealtherty.Cli.CharityCommission.Api;
using Wealtherty.Cli.CharityCommission.Graph.Model;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.GraphDb;
using Appointment = Wealtherty.Cli.CharityCommission.Graph.Model.Appointment;

namespace Wealtherty.Cli.CharityCommission;

public class Facade
{
    private readonly IDriver _driver;
    private readonly Client _client;
    private readonly CompaniesHouse.Facade _companiesHouseFacade;

    public Facade(IDriver driver, Client client, CompaniesHouse.Facade companiesHouseFacade)
    {
        _driver = driver;
        _client = client;
        _companiesHouseFacade = companiesHouseFacade;
    }

    public async Task ModelCharityAsync(string number, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        
        var charity = await _client.GetDetailsAsync(number);
        var charityNode = new Chairty(charity);

        var trustees = await _client.GetTrusteesAsync(number);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);
            
            charityNode.AddRelation(new Appointment(charityNode, trusteeNode, trustee));
        }

        if (charityNode.CompanyHouseNumber != null)
        {
            var company = await _companiesHouseFacade.ModelCompanyAsync(charityNode.CompanyHouseNumber, cancellationToken);
            
            charityNode.AddRelation(new Relationship<Chairty, Company>(charityNode, company, "HAS_COMPANY"));
        }
        await session.ExecuteCommandsAsync(charityNode);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);

            if (trustee.CharityNumber == null) continue;
            
            var otherCharity = await _client.GetDetailsAsync(trustee.CharityNumber);
            var otherCharityNode = new Chairty(otherCharity);
                
            otherCharityNode.AddRelation(new Appointment(otherCharityNode, trusteeNode, trustee));
            
            if (otherCharityNode.CompanyHouseNumber != null)
            {
                var company = await _companiesHouseFacade.ModelCompanyAsync(otherCharityNode.CompanyHouseNumber, cancellationToken);
            
                otherCharityNode.AddRelation(new Relationship<Chairty, Company>(otherCharityNode, company, "HAS_COMPANY"));
            }

            await session.ExecuteCommandsAsync(otherCharityNode);
        }
    }
}