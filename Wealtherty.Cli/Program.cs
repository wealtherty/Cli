using CommandLine;
using Wealtherty.Cli.CompaniesHouse.Commands;
using Wealtherty.Cli.Core;

namespace Wealtherty.Cli;

public static class Program
{
    private static int Main(string[] args) {
        return Parser.Default.ParseArguments<GetCompany>(args)
            .MapResult(
                Execute,
                _ => 1);
    }

    private static int Execute(Command command)
    {
        command.ExecuteAsync(new ServiceProviderFactory()).Wait();
        
        return 0;
    }
}