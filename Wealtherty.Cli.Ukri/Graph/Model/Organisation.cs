using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.Ukri.Graph.Model;

public class Organisation : Node
{
    public string OrganisationId { get; set; }
    
    public string Name { get; set; }

}