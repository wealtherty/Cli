using System.Globalization;
using CommandLine;
using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Wealtherty.Cli.CompaniesHouse.Graph.Model;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.GraphDb;
using ThinkTank = Wealtherty.Cli.Bridge.Csv.Model.ThinkTank;

namespace Wealtherty.Cli.Bridge.Commands;

[Verb("spike")]
public class Spike : Command
{
    [Option('p', "path", Default = "v2_thinktanks.csv")]
    public string Path { get; set; }
    
    [Option('d', "deleteAll", Default = true)]
    public bool DeleteAll { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var session = serviceProvider.GetService<IAsyncSession>();
        var companiesHouse = serviceProvider.GetService<CompaniesHouse.Facade>();
        var charityCommission = serviceProvider.GetService<CharityCommission.Facade>();

        using var reader = new StreamReader(Path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        if (DeleteAll)
        {
            await session.DeleteAllAsync();
        }

        var thinkTanks =
            csv.GetRecords<ThinkTank>();
        
        foreach (var thinkTank in thinkTanks)
        {
            var thinkTankNode = new ThinkTanks.Graph.Model.ThinkTank
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