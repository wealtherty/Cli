using CommandLine;
using CompaniesHouse;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("companieshouser:company-get")]
public class GetCompany : Command
{
    [Option('n', "number", Required = true)]
    public string Number { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var companiesHouseClient = serviceProvider.GetService<ICompaniesHouseClient>();

        var respone = await companiesHouseClient.GetCompanyProfileAsync(Number);
        
        Log.Information("{@Response}", respone);
    }
}