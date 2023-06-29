using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.CompaniesHouse.Model;

public enum PoliticalWing
{
    Left,
    Right
}

public class ThinkTank : Node
{
    public string Name { get; set; }
    
    public string Wing { get; set; }
}