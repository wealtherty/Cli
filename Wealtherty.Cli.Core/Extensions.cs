﻿using Microsoft.Extensions.Configuration;
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

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> self)
        {
            return self ?? Enumerable.Empty<T>();
        }
        
        public static async Task<TNode> GetNode<TNode>(this IAsyncSession self, string query) where TNode : Node
        {
            try
            {
                var result = await self.ExecuteReadAsync(async tx =>
                {
                    var resultCursor = await tx.RunAsync(query);
                    var record = await resultCursor.SingleAsync();
                    var node = record[0].As<TNode>();
                    return node;
                });

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred getting node - Query: {query}", query);
                throw;
            }
        }
    }
}