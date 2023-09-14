using Wealtherty.ThinkTanks.Graph.Model;

namespace Wealtherty.Cli.Bridge.Model.Csv;

public class PoliticalSicCodeDescription
{
    public PoliticalWing PoliticalWing { get; set; }
        
    public string SicCode { get; set; }
        
    public string SicCodeCategory { get; set; }
        
    public string SicCodeDescription { get; set; }
        
    public int Count { get; set; }
        
}