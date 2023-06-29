using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Ukri.Api;

namespace Wealtherty.Cli.Ukri
{
    public static class Extensions
    {
        public static IServiceCollection AddUkri(this IServiceCollection self)
        {
            return self.AddSingleton<Client>()
                .AddAutoMapper(typeof(Extensions).Assembly);
        }
        
    }
}