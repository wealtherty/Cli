using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Officer : Node
{
    public string OfficerId { get; set; }
    
    public string Name { get; set; }
    
    public string Nationality { get; set; }
    
    public int? YearOfBirth { get; set; }
    
    protected override object GetMatchObject() => new { OfficerId };
}