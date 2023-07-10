using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.CharityCommission.Api;
using Wealtherty.Cli.CharityCommission.Mapping;

namespace Wealtherty.Cli.CharityCommission;

public static class Extensions
{
    public static IServiceCollection AddCharityCommission(this IServiceCollection self, IConfigurationRoot configuration)
    {
        return self
            .Configure<Settings>(configuration.GetSection("CharityCommission"))
            .AddAutoMapper(typeof(Profile).Assembly)
            .AddSingleton<Client>()
            .AddSingleton<CharityCommissionFacade>();
    }
}