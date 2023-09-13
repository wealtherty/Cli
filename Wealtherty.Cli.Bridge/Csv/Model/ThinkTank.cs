using CsvHelper.Configuration.Attributes;
using Wealtherty.ThinkTanks.Graph.Model;

namespace Wealtherty.Cli.Bridge.Csv.Model;

public class ThinkTank
{
    public string OttId { get; set; }
    
    public string Name { get; set; }
    
    [Format("d/M/yyyy")]
    public DateTime FoundedOn { get; set; }

    public string Website { get; set; }
 
    public PoliticalWing PoliticalWing { get; set; }
    
}