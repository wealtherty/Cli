using Neo4j.Driver;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse;

public class Bootstrapper : IStartable
{
    private readonly IAsyncSession _session;

    public Bootstrapper(IAsyncSession session)
    {
        _session = session;
    }

    public Task StartAsync()
    {
        return Task.WhenAll(
            _session.LogAndRunAsync("CREATE CONSTRAINT officer_OfficerId IF NOT EXISTS FOR (n:Officer) REQUIRE n.OfficerId IS UNIQUE"),
            _session.LogAndRunAsync("CREATE CONSTRAINT company_Number IF NOT EXISTS FOR (n:Company) REQUIRE n.Number IS UNIQUE"));
    }
}