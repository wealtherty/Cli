using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CharityCommission.Graph.Model;

public class Charity : Node
{
    public Charity(Api.Model.Charity charity)
    {
        OrganisationNumber = charity.OrganisationNumber;
        Number = charity.Number;
        Name = charity.GetFormattedName();
        Type = charity.Type;
        RegisteredOn = charity.RegisteredOn;
        RemovedOn = charity.RemovedOn;
        LatestIncome = charity.LatestIncome;
        LatestExpenditure = charity.LatestExpenditure;
        CompanyHouseNumber = charity.CompanyHouseNumber?.PadLeft(8, '0');
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