using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wealtherty.Cli.Bridge;

public static class Extensions
{
    public static IServiceCollection AddBridge(this IServiceCollection self, IConfigurationRoot configuration)
    {
        return self;
    }
    
}