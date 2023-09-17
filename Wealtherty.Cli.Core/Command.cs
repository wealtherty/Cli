using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Wealtherty.Cli.Core;

public abstract class Command
{
    [Option('l', "loglevel", Default = LogEventLevel.Information)]
    public LogEventLevel LogLevel { get; set; }

    protected abstract Task ExecuteImplAsync(IServiceProvider serviceProvider);
    
    public async Task ExecuteAsync(IServiceProviderFactory serviceProviderFactory)
    {
        InitialiseLogging();
        
        var serviceProvider = serviceProviderFactory.Create();
        
        Log.Information("Executing {@Command}", this);
        
        try
        {
            await RunStartablesAsync(serviceProvider);
            await ExecuteImplAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred executing command");
        }
    }

    private static async Task RunStartablesAsync(IServiceProvider serviceProvider)
    {
        var startables = serviceProvider.GetServices<IStartable>();

        foreach (var startable in startables)
        {
            await startable.StartAsync();
        }
    }

    private void InitialiseLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Log.txt")
            .MinimumLevel.Is(LogLevel)
            .CreateLogger();
    }
}