using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CharityCommission.Commands;

[Verb("cc:charity-get")]
public class GetCharity : Command
{
    [Option('n', "charityNumber", Required = true)]
    public string CharityNumber { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetService<Facade>();

        await client.ModelCharityAsync(CharityNumber, new CancellationToken());
    }
}