using Wealtherty.ThinkTanks.Graph.Model;

namespace Wealtherty.Cli.Bridge.Csv.Model;

public class ThinkTank
{
    public string Name { get; set; }
    
    public PoliticalWing Wing { get; set; }
    
    public string CompanyNumber { get; set; }
    
    public string CharityNumber { get; set; }
}