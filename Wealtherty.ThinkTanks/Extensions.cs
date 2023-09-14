using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.ThinkTanks.Resources;

namespace Wealtherty.ThinkTanks;

public static class Extensions
{
    public static IServiceCollection AddThinkTanks(this IServiceCollection self, IConfigurationRoot configuration)
    {
        return self.AddSingleton<ResourceReader>();
    }
}