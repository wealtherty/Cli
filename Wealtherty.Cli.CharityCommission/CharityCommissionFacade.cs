using AutoMapper;
using Neo4j.Driver;
using Wealtherty.Cli.CharityCommission.Api;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CharityCommission;

public class CharityCommissionFacade
{
    private readonly IDriver _driver;
    private readonly IMapper _mapper;
    private readonly Client _client;

    public CharityCommissionFacade(IDriver driver, Client client, IMapper mapper)
    {
        _driver = driver;
        _client = client;
        _mapper = mapper;
    }

    public async Task ModelCharityAsync(string number)
    {
        await using var session = _driver.AsyncSession();
        
        var charity = await _client.GetDetailsAsync(number);
        var charityNode = _mapper.Map<Graph.Model.Chairty>(charity);
        
        await session.ExecuteCommandsAsync(charityNode);
        
    }
}