using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("hello-world")]
public class HelloWorld : Command
{
    protected override Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetService<IOptions<Settings>>().Value;
        
        return Task.Run(() => Log.Information(settings.Message));
    }
}