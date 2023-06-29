using CommandLine;
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
            await ExecuteImplAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred executing command");
        }
    }

    private void InitialiseLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Is(LogLevel)
            .CreateLogger();
    }
}