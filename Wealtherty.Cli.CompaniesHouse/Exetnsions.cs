using CompaniesHouse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Wealtherty.Cli.CompaniesHouse
{
    public static class Exetnsions
    {
        public static IServiceCollection AddCompaniesHouse(this IServiceCollection self, IConfigurationRoot configuration)
        {
            return self
                .Configure<Settings>(configuration.GetSection("CompaniesHouse"))
                .AddAutoMapper(typeof(Settings).Assembly)
                .AddSingleton<ICompaniesHouseSettings>(provider =>
                    new CompaniesHouseSettings(provider.GetService<IOptions<Settings>>().Value.ApiKey))
                .AddSingleton<ICompaniesHouseClient>(provider =>
                    new CompaniesHouseClient(provider.GetService<ICompaniesHouseSettings>()));
        }
    }
}