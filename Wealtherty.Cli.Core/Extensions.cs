using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Wealtherty.Cli.Core.Logging;

namespace Wealtherty.Cli.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection self)
        {
            return self.AddScoped<IStartable, LoggingStartable>();
        }
        
        public static string ToJson(this object self)
        {
            var serializer = new JsonSerializer();
            using var stringWriter = new StringWriter();
            using var writer = new JsonTextWriter(stringWriter)
            {
                QuoteName = false
            };
            serializer.Serialize(writer, self);

            return stringWriter.ToString();
        }

    }
}