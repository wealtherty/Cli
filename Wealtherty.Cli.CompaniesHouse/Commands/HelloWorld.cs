using CommandLine;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli.CompaniesHouse.Commands;

[Verb("hello-world")]
public class HelloWorld : Command
{
    protected override Task ExecuteImplAsync()
    {
        return Task.Run(() => Console.WriteLine("Hello World"));
    }
}