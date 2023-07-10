using Newtonsoft.Json;

namespace Wealtherty.Cli.CharityCommission.Api.Model;

public class Chairty
{
    [JsonProperty("organisation_number")]
    public string OrganisationNumber { get; set; }
    
    [JsonProperty("reg_charity_number")]
    public string Number { get; set; }
    
    [JsonProperty("charity_name")]
    public string Name { get; set; }
    
    [JsonProperty("charity_type")]
    public string Type { get; set; }
    
    [JsonProperty("date_of_registration")]
    public DateTime RegisteredOn { get; set; }
    
    [JsonProperty("date_of_removal")]
    public DateTime? RemovedOn { get; set; }
    
    [JsonProperty("latest_income")]
    public long? LatestIncome { get; set; }
    
    [JsonProperty("latest_expenditure")]
    public long? LatestExpenditure { get; set; }
    
    [JsonProperty("charity_co_reg_number")]
    public string CompanyHouseNumber { get; set; }
}