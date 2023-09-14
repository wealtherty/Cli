using CompaniesHouse.Response.CompanyProfile;
using Newtonsoft.Json;
using Wealtherty.Cli.Core.GraphDb;
using Wealtherty.Cli.Core.GraphDb.Converters;

namespace Wealtherty.Cli.CompaniesHouse.Model.Graph;

public class Company : Node
{
    public string Number { get; set; }
    
    public string Name { get; set; }
    
    public string Status { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
    
    [JsonConverter(typeof(DateConverter))]
    public DateTime? DateOfCreation { get; set; }

    [JsonConverter(typeof(DateConverter))]
    public DateTime? DateOfCessation { get; set; }

    public bool? HasInsolvencyHistory { get; set; }

    public bool? HasCharges { get; set; }

    public bool? HasBeenLiquidated { get; set; }

    public bool? IsCommunityInterestCompany { get; set; }
    
    [JsonIgnore]
    public CompanyProfile Resource { get; }

    public Company() { }

    public Company(string number)
    {
        Number = number;
    }

    public Company(CompanyProfile resource)
    {
        Resource = resource;
        
        Name = resource.GetFormattedName();
        Number = resource.CompanyNumber;
        Status = resource.CompanyStatus.ToString();
        DateOfCreation = resource.DateOfCreation;
        DateOfCessation = resource.DateOfCessation;
        IsCommunityInterestCompany = resource.IsCommunityInterestCompany;
        HasBeenLiquidated = resource.HasBeenLiquidated;
        HasCharges = resource.HasCharges;
        HasInsolvencyHistory = resource.HasInsolvencyHistory;
    }
    
    protected override object GetMatchObject() => new { Number };
}