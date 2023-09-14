using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Wealtherty.Cli.Core;
using Wealtherty.ThinkTanks.Resources;

namespace Wealtherty.ThinkTanks.Commands;

[Verb("tt:thinktanks-import")]
public class ImportThinkTanks : Command
{
    [Option('d', "deleteAll", Default = true)]
    public bool DeleteAll { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var session = serviceProvider.GetRequiredService<IAsyncSession>();
        var reader = serviceProvider.GetRequiredService<Reader>();
        
        if (DeleteAll)
        {
            await session.DeleteAllAsync();
        }

        var thinkTanks = reader.GetThinkTanks();
        
        foreach (var thinkTank in thinkTanks)
        {
            var thinkTankNode = new Graph.Model.ThinkTank
            {
                OttId = thinkTank.OttId,
                Name = thinkTank.Name,
                FoundedOn = thinkTank.FoundedOn,
                Website = thinkTank.Website,
                Wing = thinkTank.PoliticalWing.ToString()
            };
            
            await session.ExecuteCommandsAsync(thinkTankNode);
        }
    }
}