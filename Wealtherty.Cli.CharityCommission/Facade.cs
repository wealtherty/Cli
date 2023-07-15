using Neo4j.Driver;
using Wealtherty.Cli.CharityCommission.Api;
using Wealtherty.Cli.CharityCommission.Graph.Model;
using Wealtherty.Cli.Core;
using Appointment = Wealtherty.Cli.CharityCommission.Graph.Model.Appointment;

namespace Wealtherty.Cli.CharityCommission;

public class Facade
{
    private readonly Client _client;
    private readonly IAsyncSession _session;

    public Facade(Client client, IAsyncSession session)
    {
        _client = client;
        _session = session;
    }

    public async Task ModelCharityAsync(string number, CancellationToken cancellationToken)
    {
        var charity = await _client.GetDetailsAsync(number, cancellationToken);
        var charityNode = new Charity(charity);

        var trustees = await _client.GetTrusteesAsync(number, cancellationToken);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);
            
            charityNode.AddRelation(new Appointment(charityNode, trusteeNode, trustee));
        }

        await _session.ExecuteCommandsAsync(charityNode);
        
        foreach (var trustee in trustees)
        {
            var trusteeNode = new Trustee(trustee);

            if (trustee.CharityNumber == null) continue;
            
            var otherCharity = await _client.GetDetailsAsync(trustee.CharityNumber, cancellationToken);
            var otherCharityNode = new Charity(otherCharity);
                
            otherCharityNode.AddRelation(new Appointment(otherCharityNode, trusteeNode, trustee));
            
            await _session.ExecuteCommandsAsync(otherCharityNode);
        }
    }

    public async Task<Charity[]> GetCharitiesAsync()
    {
        return await _session.GetNodesAsync<Charity>("MATCH (c:Charity) return c");
    }
}