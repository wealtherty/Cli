using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.CharityCommission.Api;

namespace Wealtherty.Cli.CharityCommission;

public static class Extensions
{
    public static IServiceCollection AddCharityCommission(this IServiceCollection self, IConfigurationRoot configuration)
    {
        return self
            .Configure<Settings>(configuration.GetSection("CharityCommission"))
            .AddSingleton<Client>()
            .AddSingleton<Facade>();
    }
    
    public static string GetFormattedName(this Api.Model.Charity self)
    {
        return self.Name.ToLower().Transform(To.TitleCase);
    }

    public static string GetFormattedName(this Api.Model.Trustee self)
    {
        return self.Name.ToLower().Transform(To.TitleCase);
    }
}