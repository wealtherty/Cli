using CsvHelper.Configuration.Attributes;
using Wealtherty.ThinkTanks.Model.Graph;

namespace Wealtherty.ThinkTanks.Model.Csv;

public class ThinkTank
{
    public string OttId { get; set; }
    
    public string Name { get; set; }
    
    [Format("d/M/yyyy")]
    public DateTime FoundedOn { get; set; }

    public string Website { get; set; }
 
    public PoliticalWing PoliticalWing { get; set; }
    
}