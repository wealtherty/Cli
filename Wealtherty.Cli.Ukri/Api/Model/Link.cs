namespace Wealtherty.Cli.Ukri.Api.Model;

public class Link
{
    public string Href { get; set; }
    
    public string Rel { get; set; }

    public string GetId() => Href.Split('/').Last();
}