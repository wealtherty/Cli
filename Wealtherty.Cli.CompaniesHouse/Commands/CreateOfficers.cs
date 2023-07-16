using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("ch:officers-create")]
public class CreateOfficers : Command
{
    [Option('n', "number")]
    
    public string CompanyNumber { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var facade = serviceProvider.GetService<Facade>();

        await facade.DeleteAllAsync();
        await facade.CreateOfficersAsync(CompanyNumber, new CancellationToken());
    }
}