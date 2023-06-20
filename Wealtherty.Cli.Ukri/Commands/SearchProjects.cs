using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wealtherty.Cli.Core;
using Wealtherty.Cli.Ukri.Api;

namespace Wealtherty.Cli.Ukri.Commands;

[Verb("ukri:projects-search")]
public class SearchProjects : Command
{
    [Option('q', "query", Required = true)]
    public string Query { get; set; }
    
    protected override async Task ExecuteImplAsync(IServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetService<Client>();

        var projects = await client.SearchProjectsAsync(Query, 50);
        
        Log.Information("Projects: {@Projects}", projects);
    }
}