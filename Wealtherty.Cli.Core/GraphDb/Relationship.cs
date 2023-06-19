using Newtonsoft.Json;

namespace Wealtherty.Cli.Core.GraphDb;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public abstract class Relationship
{
    [JsonIgnore]
    public Node Parent { get; set; }
    
    [JsonIgnore]
    public Node Child { get; set; }
    
    public abstract string GetMergeCommand();
}

public class Relationship<TParent, TChild> : Relationship where TParent : Node where TChild: Node
{
    protected Relationship(TParent parent, TChild child)
    {
        Parent = parent;
        Child = child;
    }
    
    protected virtual string GetName() => GetType().Name;


    public override string GetMergeCommand()
    {
        var parent = Parent.GetMatchExpression('p');
        var child = Child.GetMatchExpression('c');
        var relationship = GetRelationshipExpression('r');
        
        return $"MATCH {parent}, {child} MERGE (p)-{relationship}->(c) RETURN r;";
    }
    
    private string GetRelationshipExpression(char prefix) => $"[{prefix}:{GetName()} {GetRelationshipObject().ToJson()}]";
    
    private object GetRelationshipObject() => this;
}
