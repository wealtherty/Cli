using Microsoft.Extensions.DependencyInjection;

namespace Wealtherty.Cli.Core;

public abstract class Command
{
    protected abstract Task ExecuteImplAsync(IServiceProvider serviceProvider);
    
    public async Task ExecuteAsync(IServiceProviderFactory serviceProviderFactory)
    {
        var serviceProvider = serviceProviderFactory.Create();

        var startables = serviceProvider.GetService<IEnumerable<IStartable>>();

        foreach (var startable in startables)
        {
            startable.Start();
        }
            
        await ExecuteImplAsync(serviceProvider);
    }
}