using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.Ukri.Graph.Model;

public class NamedRelationship<TParent, TChild> : Relationship<TParent, TChild> where TParent : Node where TChild: Node
{
    private readonly string _name;
    
    public NamedRelationship(TParent parent, TChild child, string name) : base(parent, child)
    {
        _name = name;
    }

    protected override string GetName() => _name;
}