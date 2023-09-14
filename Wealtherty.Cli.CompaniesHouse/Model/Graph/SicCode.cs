using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Model.Graph;

public class SicCode : Node
{
    public string Code { get; set; }
    
    public string Category { get; set; }
    
    public string Description { get; set; }
}