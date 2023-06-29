using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Officer : Node
{
    public string OfficerId { get; set; }
    
    public string Name { get; set; }
    
    public string Nationality { get; set; }
    public int? YearOfBirth { get; set; }
    
    [JsonIgnore]
    public global::CompaniesHouse.Response.Officers.Officer Resource { get; }
    
    public Officer(global::CompaniesHouse.Response.Officers.Officer resource)
    {
        Resource = resource;
        
        Name = resource.GetFormattedName();
        OfficerId = resource.Links.Officer.OfficerId;
        YearOfBirth = resource.DateOfBirth?.Year;
    }
    
    
    protected override object GetMatchObject() => new { OfficerId };
}