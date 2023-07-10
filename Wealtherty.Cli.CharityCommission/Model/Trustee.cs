using Newtonsoft.Json;

namespace Wealtherty.Cli.CharityCommission.Model;

public class Trustee
{
    public string Name { get; set; }
    
    [JsonProperty("is_chair")]
    public bool IsChair { get; set; }
    
    [JsonProperty("date_of_appointment")]
    public DateTime? AppointedOn { get; set; }
    
    [JsonProperty("organisation_number")]
    public string OrganisationNumber { get; set; }
    
    [JsonProperty("charity_name")]
    public string CharityName { get; set; }
    
    [JsonProperty("reporting_status")]
    public string ReportingStatus { get; set; }
    
    [JsonProperty("reg_charity_number")]
    public string CharityNumber { get; set; }
}