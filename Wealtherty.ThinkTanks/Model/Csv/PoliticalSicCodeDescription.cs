using Wealtherty.ThinkTanks.Model.Graph;

namespace Wealtherty.ThinkTanks.Model.Csv;

public class PoliticalSicCodeDescription
{
    public PoliticalWing PoliticalWing { get; set; }
        
    public string SicCode { get; set; }
        
    public string SicCodeCategory { get; set; }
        
    public string SicCodeDescription { get; set; }
        
    public int Count { get; set; }
        
}