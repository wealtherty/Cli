using Microsoft.Extensions.DependencyInjection;

namespace Wealtherty.Cli.Core;

public abstract class Command
{
    protected abstract Task ExecuteImplAsync(IServiceProvider serviceProvider);
    
    public async Task ExecuteAsync(IServiceProviderFactory serviceProviderFactory)
    {
        var serviceProvider = serviceProviderFactory.Create();

        ExecuteStartable(serviceProvider);

        await ExecuteImplAsync(serviceProvider);
    }

    private static void ExecuteStartable(IServiceProvider serviceProvider)
    {
        var startables = serviceProvider.GetService<IEnumerable<IStartable>>();

        foreach (var startable in startables)
        {
            startable.Execute();
        }
    }
}