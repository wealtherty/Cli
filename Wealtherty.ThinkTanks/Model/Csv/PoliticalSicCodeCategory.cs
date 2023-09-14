using Wealtherty.ThinkTanks.Model.Graph;

namespace Wealtherty.ThinkTanks.Model.Csv;

public class PoliticalSicCodeCategory
{
    public PoliticalWing PoliticalWing { get; set; }
        
    public string SicCodeCategory { get; set; }
        
    public int Count { get; set; }
}