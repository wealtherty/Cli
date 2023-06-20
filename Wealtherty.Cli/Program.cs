using CommandLine;
using Wealtherty.Cli.CompaniesHouse.Commands;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Ukri.Commands;

namespace Wealtherty.Cli;

public static class Program
{
    private static int Main(string[] args) {
        return Parser.Default.ParseArguments<GetCompany, SearchProjects>(args)
            .MapResult(
                (GetCompany command) => Execute(command),
                (SearchProjects command) => Execute(command),
                _ => 1);
    }

    private static int Execute(Command command)
    {
        command.ExecuteAsync(new ServiceProviderFactory()).Wait();
        
        return 0;
    }
}