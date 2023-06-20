using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Newtonsoft.Json;
using Serilog;
using Wealtherty.Cli.Core.GraphDb;
using Wealtherty.Cli.Core.Logging;

namespace Wealtherty.Cli.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection self, IConfigurationRoot configuration)
        {
            return self
                .Configure<Settings>(configuration.GetSection("Neo4j"))
                .AddScoped<IStartable, LoggingStartable>()
                .AddSingleton(provider =>
                {
                    var settings = provider.GetService<IOptions<Settings>>().Value;

                    return GraphDatabase.Driver(settings.Uri, AuthTokens.Basic(settings.Username, settings.Password));
                });
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
        
        public static async Task ExecuteCommandsAsync(this IAsyncSession self, Node node)
        {
            var commands = node.GetCommands();
            
            foreach (var command in commands)
            {
                Log.Debug(command);
                await self.RunAsync(command);
            }
        }
    }
}