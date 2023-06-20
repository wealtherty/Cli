using Newtonsoft.Json;

namespace Wealtherty.Cli.Ukri.Api.Model;

public class LinksWrapper
{
    [JsonProperty("Link")] 
    public Link[] Links { get; set; } = Array.Empty<Link>();
}