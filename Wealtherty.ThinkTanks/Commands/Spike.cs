using System.Globalization;
using CommandLine;
using CsvHelper;
using Serilog;
using Wealtherty.Cli.Core;

namespace Wealtherty.ThinkTanks.Commands;

[Verb("spike")]
public class Spike : Command
{
    [Option('p', "path", Default = "ThinkTanks.csv")]
    public string Path { get; set; }
    
    protected override Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        using var reader = new StreamReader(Path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var thinkTanks =
            csv
                .GetRecords<Model.Csv.ThinkTank>()
                .GroupBy(x => new { x.Name, x.Wing },
                    tank => new { tank.CompanyNumber, tank.CharityNumber });
        
        foreach (var thinkTank in thinkTanks)
        {
            Log.Information("{@ThinkTank}", thinkTank.Key);

            foreach (var blah in thinkTank)
            {
                Log.Information("{@Blah}", blah);
            }
        }
        
        return Task.CompletedTask;
    }
}