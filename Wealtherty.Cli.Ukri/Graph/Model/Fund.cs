using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.Ukri.Graph.Model;

public class Fund : Node
{
    public string FundId { get; set; }
    
    public string Category { get; set; }
 
    public int Amount { get; set; }
}