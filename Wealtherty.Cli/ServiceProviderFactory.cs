using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Core.Logging;

namespace Wealtherty.Cli;

public class ServiceProviderFactory : IServiceProviderFactory
{
    public IServiceProvider Create()
    {
        return new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

    }
}