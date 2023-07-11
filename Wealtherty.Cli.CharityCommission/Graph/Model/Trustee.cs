using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CharityCommission.Graph.Model;

public class Trustee : Node
{
    public Trustee(Api.Model.Trustee trustee)
    {
        Name = trustee.GetFormattedName();
    }

    public string Name { get; set; }
}