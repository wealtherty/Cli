using Serilog;
using Serilog.Events;

namespace Wealtherty.Cli.Core.Logging;

public class LoggingStartable : IStartable
{
    public void Execute()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Is(LogEventLevel.Information)
            .CreateLogger();
    }
}