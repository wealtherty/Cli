using Neo4j.Driver;
using Wealtherty.Cli.CharityCommission.Api;
using Wealtherty.Cli.CharityCommission.Graph.Model;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CharityCommission;

public class Facade
{
    private readonly IDriver _driver;
    private readonly Client _client;

    public Facade(IDriver driver, Client client)
    {
        _driver = driver;
        _client = client;
    }

    public async Task ModelCharityAsync(string number)
    {
        await using var session = _driver.AsyncSession();
        
        var charity = await _client.GetDetailsAsync(number);
        var charityNode = new Chairty(charity);

        var trustees = await _client.GetTrusteesAsync(number);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);
            
            charityNode.AddRelation(new Relationship<Chairty,Trustee>(charityNode, trusteeNode, "HAS_TRUSTEE"));
        }
        await session.ExecuteCommandsAsync(charityNode);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);

            if (trustee.CharityNumber == null) continue;
            
            var otherCharity = await _client.GetDetailsAsync(trustee.CharityNumber);
            var otherCharityNode = new Chairty(otherCharity);
                
            otherCharityNode.AddRelation(new Relationship<Chairty,Trustee>(otherCharityNode, trusteeNode, "HAS_TRUSTEE"));
            
            await session.ExecuteCommandsAsync(otherCharityNode);
        }
    }
}