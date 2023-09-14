using CommandLine;
using Wealtherty.Cli.Bridge.Commands;
using Wealtherty.Cli.CharityCommission.Commands;
using Wealtherty.Cli.CompaniesHouse.Commands;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Ukri.Commands;
using Wealtherty.ThinkTanks.Commands;

namespace Wealtherty.Cli;

public static class Program
{
    private static int Main(string[] args) {
        return Parser.Default.ParseArguments<GetCompanies, GetCompany, SearchProjects, GetCharity, ConnectCharitiesAndCompanies, ImportThinkTanks, CreateOfficers, GetThinkTanksAppointments, AnalyseThinkTanksAppointments>(args)
            .MapResult(
                (GetCompanies command) => Execute(command),
                (GetCompany command) => Execute(command),
                (SearchProjects command) => Execute(command),
                (GetCharity command) => Execute(command),
                (ConnectCharitiesAndCompanies command) => Execute(command),
                (ImportThinkTanks command) => Execute(command),
                (CreateOfficers command) => Execute(command),
                (GetThinkTanksAppointments command) => Execute(command),
                (AnalyseThinkTanksAppointments command) => Execute(command),
                _ => 1);
    }

    private static int Execute(Command command)
    {
        command.ExecuteAsync(new ServiceProviderFactory()).Wait();
        
        return 0;
    }
}