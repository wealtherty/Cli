using Newtonsoft.Json;

namespace Wealtherty.Cli.Core.GraphDb;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public abstract class Node
{
    private readonly IList<Relationship> _relationships;

    protected Node() => _relationships = new List<Relationship>();
    
    public string GetMatchExpression(char prefix) => $"({prefix}:{GetType().Name} {GetMatchObject().ToJson()})";
    
    public void AddRelation(Relationship relationship) => _relationships.Add(relationship);
    public void AddRelations(IEnumerable<Relationship> relationships)
    {
        foreach (var relationship in relationships)
        {
            AddRelation(relationship);
        }
    }

    public string[] GetCommands()
    {
        var commands = new List<string> { GetMergeCommand() };

        commands.AddRange(_relationships.SelectMany(relationship => relationship.Child.GetCommands()));
        
        commands.AddRange(_relationships.Select(relationship => relationship.GetMergeCommand()));

        return commands.ToArray();
    }

    protected virtual object GetMatchObject() => this;

    public virtual object GetRelationshipObject() => throw new NotImplementedException();

    private string GetMergeCommand() => $"MERGE ({'n'}:{GetType().Name} {GetMergeObject().ToJson()}) RETURN n;";

    private object GetMergeObject() => this;
}