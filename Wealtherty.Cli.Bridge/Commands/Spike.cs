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
    [Option('p', "path", Default = "ThinkTanks.csv")]
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
            csv
                .GetRecords<ThinkTank>()
                .GroupBy(x => new { x.Name, x.Wing },
                    tank => new { tank.CompanyNumber, tank.CharityNumber });
        
        foreach (var thinkTank in thinkTanks)
        {
            var thinkTankNode = new ThinkTanks.Graph.Model.ThinkTank
            {
                Name = thinkTank.Key.Name,
                Wing = thinkTank.Key.Wing.ToString()
            };
            
            foreach (var references in thinkTank)
            {
                if (!string.IsNullOrEmpty(references.CompanyNumber))
                {
                    var companyNode = await companiesHouse.CreateOfficersAndCompaniesAsync(references.CompanyNumber, CancellationToken.None);
                    
                    thinkTankNode.AddRelation(new Relationship<ThinkTanks.Graph.Model.ThinkTank,Company>(thinkTankNode, companyNode, "HAS_COMPANY"));
                }
                // if (!string.IsNullOrEmpty(references.CharityNumber))
                // {
                //     var charityNode = await charityCommission.ModelCharityAsync(references.CharityNumber, CancellationToken.None);
                //     thinkTankNode.AddRelation(new Relationship<ThinkTanks.Graph.Model.ThinkTank,Charity>(thinkTankNode, charityNode, "HAS_CHARITY"));
                // }
            }
            
            await session.ExecuteCommandsAsync(thinkTankNode);
        }
    }
}