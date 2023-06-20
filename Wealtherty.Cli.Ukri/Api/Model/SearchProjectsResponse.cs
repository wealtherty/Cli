using Newtonsoft.Json;

namespace Wealtherty.Cli.Ukri.Api.Model;

public class SearchProjectsResponse
{
    [JsonProperty("Project")]
    public Project[] Projects { get; set; } = Array.Empty<Project>();
    
    public int TotalPages { get; set; }
}