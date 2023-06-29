namespace Wealtherty.Cli.Ukri.Api.Model;

public class Link
{
    public string Href { get; set; }
    
    public string Rel { get; set; }

    public string GetId() => Href.Split('/').Last();

    public bool IsForOrganisation() => Href.Contains("gtr/api/organisations", StringComparison.OrdinalIgnoreCase);

    public bool IsForPerson() => Href.Contains("gtr/api/persons", StringComparison.OrdinalIgnoreCase);
    
    public bool IsForFund() => Href.Contains("gtr/api/funds", StringComparison.OrdinalIgnoreCase);
}