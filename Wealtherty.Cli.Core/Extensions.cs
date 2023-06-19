using Microsoft.Extensions.DependencyInjection;
using Wealtherty.Cli.Core.Logging;

namespace Wealtherty.Cli.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection self)
        {
            return self.AddScoped<IStartable, LoggingStartable>();
        }
    }
}