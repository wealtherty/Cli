using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Graph.Model;

public class Officer : Node
{
    public string OfficerId { get; set; }
    
    public string Name { get; set; }
    
    [JsonIgnore]
    public global::CompaniesHouse.Response.Officers.Officer Resource { get; }
    
    public Officer(global::CompaniesHouse.Response.Officers.Officer resource)
    {
        Resource = resource;
        
        Name = resource.GetFormattedName();
        OfficerId = resource.Links.Officer.OfficerId;
    }
    
    
    protected override object GetMatchObject() => new { OfficerId };
}