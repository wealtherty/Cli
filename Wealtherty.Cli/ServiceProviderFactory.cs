using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.CompaniesHouse;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.Logging;

namespace Wealtherty.Cli;

public class ServiceProviderFactory : IServiceProviderFactory
{
    public IServiceProvider Create()
    {
        var configuration = GetConfigurationRoot();

        return new ServiceCollection()
            .AddLogging()
            .AddCompaniesHouse(configuration)
            .BuildServiceProvider();
    }
    
    private static IConfigurationRoot GetConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, false)
            .Build();
    }
}