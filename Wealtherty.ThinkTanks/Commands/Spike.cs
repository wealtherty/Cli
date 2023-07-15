using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Wealtherty.Cli.Core;
using Wealtherty.ThinkTanks.Graph.Model;

namespace Wealtherty.ThinkTanks.Commands;

[Verb("spike")]
public class Spike : Command
{
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var session = serviceProvider.GetService<IAsyncSession>();

        var thinkTank = new ThinkTank
        {
            Name = "Foo",
            Wing = PoliticalWing.Left.ToString()
        };
        
        await session.ExecuteCommandsAsync(thinkTank);
    }
}