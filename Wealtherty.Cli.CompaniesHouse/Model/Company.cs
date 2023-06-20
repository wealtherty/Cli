using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Company : Node
{
    public string Number { get; set; }
    
    public string Name { get; set; }
    
    public string Status { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
    
    public string CreatedOn { get; set; }

    public string StoppedTradingOn { get; set; }

    protected override object GetMatchObject() => new { Number };
}