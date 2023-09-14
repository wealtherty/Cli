using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Newtonsoft.Json;
using Serilog;
using Wealtherty.Cli.Core.GraphDb;

namespace Wealtherty.Cli.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection self, IConfigurationRoot configuration)
        {
            return self
                .Configure<Settings>(configuration.GetSection("Neo4j"))
                .AddSingleton(provider =>
                {
                    var settings = provider.GetService<IOptions<Settings>>().Value;

                    return GraphDatabase.Driver(settings.Uri, AuthTokens.Basic(settings.Username, settings.Password));
                })
                .AddSingleton(provider => provider.GetService<IDriver>().AsyncSession())
                .AddSingleton<InputReader>();
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
            foreach (var command in node.GetCommands())
            {
                await self.LogAndRunAsync(command);
            }
        }

        public static Task DeleteAllAsync(this IAsyncSession self) => self.LogAndRunAsync("MATCH (n) DETACH DELETE n");

        public static async Task LogAndRunAsync(this IAsyncSession self, string command)
        {
            try
            {
                Log.Debug("Running command - Command: {@Command}", command);
                await self.RunAsync(command);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred executing command - Command: {Command}", command);
            }
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> self)
        {
            return self ?? Enumerable.Empty<T>();
        }
        
        public static async Task<TNode> GetNodeAsync<TNode>(this IAsyncSession self, string query, IDictionary<string,object> parameters = null) where TNode : Node
        {
            try
            {
                var result = await self.ExecuteReadAsync(async tx =>
                {
                    var resultCursor = await tx.RunAsync(query, parameters);

                    var fetched = await resultCursor.FetchAsync();
                    if (!fetched)
                    {
                        return null;
                    }

                    var record = resultCursor.Current;
                    
                    var json = JsonConvert.SerializeObject(record[0].As<INode>().Properties);

                    return JsonConvert.DeserializeObject<TNode>(json);
                });

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred getting node - Query: {query}", query);
                throw;
            }
        }
        
        public static async Task<TNode[]> GetNodesAsync<TNode>(this IAsyncSession self, string query, IDictionary<string,object> parameters = null) where TNode : Node
        {
            try
            {
                var result = await self.ExecuteReadAsync(async tx =>
                {
                    var nodes = new List<TNode>();
                    
                    var resultCursor = await tx.RunAsync(query, parameters);

                    while (await resultCursor.FetchAsync())
                    {
                        var record = resultCursor.Current;
                        
                        var json = JsonConvert.SerializeObject(record[0].As<INode>().Properties);

                        var node = JsonConvert.DeserializeObject<TNode>(json);
                        
                        nodes.Add(node);
                    }

                    return nodes;
                });

                return result.OrEmpty().ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred getting node - Query: {query}", query);
                throw;
            }
        }
    }
}