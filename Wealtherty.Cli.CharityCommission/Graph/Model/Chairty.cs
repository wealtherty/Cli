using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CharityCommission.Graph.Model;

public class Chairty : Node
{
    public string OrganisationNumber { get; set; }
    
    public string Number { get; set; }
    
    public string Name { get; set; }
    
    public string Type { get; set; }
    
    public DateTime RegisteredOn { get; set; }
    
    public DateTime? RemovedOn { get; set; }
    
    public long? LatestIncome { get; set; }
    
    public long? LatestExpenditure { get; set; }
    
    public string CompanyHouseNumber { get; set; }
}