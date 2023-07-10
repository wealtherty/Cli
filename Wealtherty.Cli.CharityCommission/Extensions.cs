using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wealtherty.Cli.CharityCommission;

public static class Extensions
{
    public static IServiceCollection AddCharityCommission(this IServiceCollection self, IConfigurationRoot configuration)
    {
        return self
            .Configure<Settings>(configuration.GetSection("CharityCommission"))
            .AddSingleton<Client>();
    }
}