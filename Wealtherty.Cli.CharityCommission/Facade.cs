using Neo4j.Driver;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CharityCommission;

public class Facade
{
    private readonly IDriver _driver;
    private readonly Client _client;

    public Facade(IDriver driver, Client client)
    {
        _driver = driver;
        _client = client;
    }

    public async Task ModelCharityAsync(string number)
    {
        await using var session = _driver.AsyncSession();
        
        var charity = await _client.GetDetailsAsync(number);
        
        await session.ExecuteCommandsAsync(charity);
        
    }
}