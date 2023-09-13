using System.Globalization;
using CommandLine;
using CsvHelper;
using Serilog;
using Wealtherty.Cli.Core;
using Wealtherty.ThinkTanks.Csv.Model;
using Wealtherty.ThinkTanks.Graph.Model;
using ThinkTank = Wealtherty.ThinkTanks.Csv.Model.ThinkTank;

namespace Wealtherty.Cli.Bridge.Commands;

[Verb("spike")]
public class Spike : Command
{
    protected override Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var thinkTanks = ReadCsvFile<ThinkTank>("v2_thinktanks.csv");
        var companies = ReadCsvFile<ThinkTankCompany>("v2_thinktanks_companies.csv");

        var fromDate = new DateTime(2000, 1, 1);
        var toDate = new DateTime(2010, 1, 1);
        
        var things = new List<Thing>();

        foreach (var thinkTank in thinkTanks)
        {
            if (thinkTank.FoundedOn > toDate) continue;

            var thinkTankCompanies = companies
                .Where(x => x.OttId == thinkTank.OttId && x.CompanyNumber != null)
                .OrEmpty()
                .ToArray();

            foreach (var company in thinkTankCompanies)
            {
                var thing = new Thing
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    PoliticalWing = thinkTank.PoliticalWing,
                    ThinkTank = thinkTank.Name,
                    CompanyNumber = company.CompanyNumber
                };
                
                things.Add(thing);
            }
        }

        foreach (var thing in things)
        {
            Log.Information("Thing: {@Thing}\r\n\r\n", thing);
        }
        
        return Task.CompletedTask;
    }

    private static IEnumerable<T> ReadCsvFile<T>(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var rows =
            csv.GetRecords<T>()
                .ToArray();

        return rows;

    }

    public class Thing
    {
        public DateTime FromDate { get; set; }
        
        public DateTime ToDate { get; set; }
        
        public PoliticalWing PoliticalWing { get; set; }
        
        public string ThinkTank { get; set; }
        
        public string CompanyNumber { get; set; }
        
        public string CompanyName { get; set; }
        
        public string Officer { get; set; }
        
        public string Role { get; set; }
        
        public DateTime? AppointedOn { get; set; }
        
        public DateTime? ResignedOn { get; set; }
        
        public string SicCode { get; set; }
    }
}