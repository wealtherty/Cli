using AutoMapper;
using CommandLine;
using CompaniesHouse;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Serilog;
using Wealtherty.Cli.CompaniesHouse.Model;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("ch:company-get")]
public class GetCompany : Command
{
    [Option('n', "number", Required = true)]
    public string Number { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var companiesHouseClient = serviceProvider.GetService<ICompaniesHouseClient>();
        var mapper = serviceProvider.GetService<IMapper>();
        var driver = serviceProvider.GetService<IDriver>();
        
        await using var session = driver.AsyncSession();

        var respone = await companiesHouseClient.GetCompanyProfileAsync(Number);
        var companyNode = mapper.Map<Company>(respone.Data);
        
        await session.ExecuteCommandsAsync(companyNode);
        
        Log.Information("{@Company}", companyNode);
    }
}