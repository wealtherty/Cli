namespace Wealtherty.Cli.Core;

public abstract class Command
{
    protected abstract Task ExecuteImplAsync();
    
    public Task ExecuteAsync()
    {
        return ExecuteImplAsync();
    }
}