using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CharityCommission.Graph.Model;

public class Chairty : Node
{
    public Chairty(Api.Model.Chairty chairty)
    {
        OrganisationNumber = chairty.OrganisationNumber;
        Number = chairty.Number;
        Name = chairty.GetFormattedName();
        Type = chairty.Type;
        RegisteredOn = chairty.RegisteredOn;
        RemovedOn = chairty.RemovedOn;
        LatestIncome = chairty.LatestIncome;
        LatestExpenditure = chairty.LatestExpenditure;
        CompanyHouseNumber = chairty.CompanyHouseNumber?.PadLeft(8, '0');
    }
    
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