using CompaniesHouse;
using CompaniesHouse.Response.CompanyProfile;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wealtherty.Cli.CompaniesHouse.Services;

namespace Wealtherty.Cli.CompaniesHouse
{
    public static class Extensions
    {
        public static IServiceCollection AddCompaniesHouse(this IServiceCollection self, IConfigurationRoot configuration)
        {
            return self
                .Configure<Settings>(configuration.GetSection("CompaniesHouse"))
                .AddSingleton<ICompaniesHouseSettings>(provider =>
                    new CompaniesHouseSettings(provider.GetService<IOptions<Settings>>().Value.ApiKey))
                .AddSingleton<ICompaniesHouseClient>(provider =>
                    new CompaniesHouseClient(provider.GetService<ICompaniesHouseSettings>()))
                .AddSingleton<Client>()
                .AddSingleton<Facade>();
        }

        public static string GetFormattedName(this global::CompaniesHouse.Response.Officers.Officer self)
        {
            var formattedName = self.Name.ToLower().Transform(To.TitleCase);
            var parts = formattedName.Split(", ");
            Array.Reverse(parts);
            var reversed = string.Join(" ", parts);
            return reversed;
        }
        
        
        public static string GetFormattedName(this CompanyProfile self)
        {
            return self.CompanyName.ToLower().Transform(To.TitleCase);
        }

    }
}