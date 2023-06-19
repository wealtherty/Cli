using CommandLine;
using Serilog;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("hello-world")]
public class HelloWorld : Command
{
    protected override Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        return Task.Run(() => Log.Information("Hello World"));
    }
}