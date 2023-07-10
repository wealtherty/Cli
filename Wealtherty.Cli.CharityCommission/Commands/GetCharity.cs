using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CharityCommission.Commands;

[Verb("cc:charity-get")]
public class GetCharity : Command
{
    [Option('n', "charityNumber", Required = true)]
    public string RegisteredNumber { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetService<Client>();

        var charity = await client.GetCharityDetailsAsync(RegisteredNumber);
        
        Log.Information("{@Charity}", charity);
    }
}