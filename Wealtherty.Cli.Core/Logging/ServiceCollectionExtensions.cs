using Microsoft.Extensions.DependencyInjection;

namespace Wealtherty.Cli.Core.Logging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection self)
        {
            return self.AddScoped<IStartable, LoggingStartable>();
        }
    }
}