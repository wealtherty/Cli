using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Bridge;
using Wealtherty.Cli.CharityCommission;
using Wealtherty.Cli.CompaniesHouse;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Ukri;
using Wealtherty.ThinkTanks;

namespace Wealtherty.Cli;

public class ServiceProviderFactory : IServiceProviderFactory
{
    public IServiceProvider Create()
    {
        var configuration = GetConfigurationRoot();

        return new ServiceCollection()
            .AddCore(configuration)
            .AddThinkTanks(configuration)
            .AddCompaniesHouse(configuration)
            .AddCharityCommission(configuration)
            .AddUkri()
            .AddBridge(configuration)
            .BuildServiceProvider();
    }
    
    private static IConfigurationRoot GetConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, false)
            .AddUserSecrets(typeof(Program).Assembly)
            .Build();
    }
}