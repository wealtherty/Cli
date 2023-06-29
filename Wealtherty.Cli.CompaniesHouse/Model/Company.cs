using CompaniesHouse.Response.CompanyProfile;
using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;
using Wealtherty.Cli.Core.GraphDb.Converters;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Company : Node
{
    public string Number { get; set; }
    
    public string Name { get; set; }
    
    public string Status { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
    
    [JsonConverter(typeof(DateConverter))]
    public DateTime? CreatedOn { get; set; }

    [JsonConverter(typeof(DateConverter))]
    public DateTime? StoppedTradingOn { get; set; }

    [JsonIgnore]
    public CompanyProfile Resource { get; }

    public Company(CompanyProfile resource)
    {
        Resource = resource;
        
        Name = resource.GetFormattedName();
        Number = resource.CompanyNumber;
        Status = resource.CompanyStatus.ToString();
        CreatedOn = resource.DateOfCessation;
        StoppedTradingOn = resource.DateOfCessation;
    }


    protected override object GetMatchObject() => new { Number };
}