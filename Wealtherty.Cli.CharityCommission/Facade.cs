using Neo4j.Driver;
using Serilog;
using Wealtherty.Cli.CharityCommission.Api;
using Wealtherty.Cli.CharityCommission.Graph.Model;
using Wealtherty.Cli.Core;
using Appointment = Wealtherty.Cli.CharityCommission.Graph.Model.Appointment;

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

    public async Task ModelCharityAsync(string number, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        
        var charity = await _client.GetDetailsAsync(number, cancellationToken);
        var charityNode = new Charity(charity);

        var trustees = await _client.GetTrusteesAsync(number, cancellationToken);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);
            
            charityNode.AddRelation(new Appointment(charityNode, trusteeNode, trustee));
        }

        await session.ExecuteCommandsAsync(charityNode);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);

            if (trustee.CharityNumber == null) continue;
            
            var otherCharity = await _client.GetDetailsAsync(trustee.CharityNumber, cancellationToken);
            var otherCharityNode = new Charity(otherCharity);
                
            otherCharityNode.AddRelation(new Appointment(otherCharityNode, trusteeNode, trustee));
            
            await session.ExecuteCommandsAsync(otherCharityNode);
        }
    }

    public async Task<Charity[]> GetCharitiesAsync()
    {
        await using var session = _driver.AsyncSession();

        return await session.GetNodesAsync<Charity>("MATCH (c:Charity) return c");
    }
}