
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.Ukri.Graph.Model;

public class Person : Node
{
    public string PersonId { get; set; }
    
    public string FirstName { get; set; }
    
    public string Surname { get; set; }
}