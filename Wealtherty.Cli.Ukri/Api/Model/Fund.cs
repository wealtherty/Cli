using Newtonsoft.Json;

namespace Wealtherty.Cli.Ukri.Api.Model;

public class Fund
{
    public string Id { get; set; }
    
    public string Category { get; set; }
 
    [JsonProperty("valuePounds")]
    public Money Value { get; set; }
}