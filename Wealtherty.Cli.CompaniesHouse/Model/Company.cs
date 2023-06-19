using Newtonsoft.Json;
using Wealtherty.Cli.CompaniesHouse.Converters;
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public class Company : Node
{
    public string Type { get; set; }

    [JsonConverter(typeof(TitleConverter))]
    public string Name { get; set; }

    public string Number { get; set; }

    public string CompanyStatus { get; set; }

    public DateTime? DateOfCreation { get; set; }

    public DateTime? DateOfCessation { get; set; }
}