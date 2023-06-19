using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Wealtherty.Cli.Core;

public abstract class Command
{
    protected abstract Task ExecuteImplAsync(IServiceProvider serviceProvider);
    
    public async Task ExecuteAsync(IServiceProviderFactory serviceProviderFactory)
    {
        var serviceProvider = serviceProviderFactory.Create();

        ExecuteStartable(serviceProvider);

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

    private static void ExecuteStartable(IServiceProvider serviceProvider)
    {
        var startables = serviceProvider.GetService<IEnumerable<IStartable>>();

        if (startables == null) return;

        foreach (var startable in startables)
        {
            startable.Execute();
        }
    }
}