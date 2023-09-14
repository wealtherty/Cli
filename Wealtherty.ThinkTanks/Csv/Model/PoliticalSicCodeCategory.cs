using Wealtherty.ThinkTanks.Graph.Model;

namespace Wealtherty.ThinkTanks.Csv.Model;

public class PoliticalSicCodeCategory
{
    public PoliticalWing PoliticalWing { get; set; }
        
    public string SicCodeCategory { get; set; }
        
    public int Count { get; set; }
}