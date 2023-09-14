using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.ThinkTanks.Model.Graph;

public class ThinkTank : Node
{
    public string OttId { get; set; }
    
    public string Name { get; set; }
    
    public DateTime FoundedOn { get; set; }
    
    public string Website { get; set; }
    
    public string Wing { get; set; }
    
}