using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wealtherty.Cli.CompaniesHouse
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCompaniesHouse(this IServiceCollection self, IConfigurationRoot configuration)
        {
            return self
                .Configure<Settings>(configuration.GetSection("CompaniesHouse"));
        }
    }
}