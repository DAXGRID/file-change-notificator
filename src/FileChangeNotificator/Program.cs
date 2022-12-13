using FileChangeNotificator.FileWatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Text.Json;

namespace FileChangeNotificator;

internal static class Program
{
    public static async Task Main()
    {
        var host = new HostBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddHostedService<FileChangeNotificatorHost>();
                services.AddLogging(l => l.AddSerilog(GetLogger()));
                services.AddSingleton<Setting>(GetSettings());
                services.AddSingleton<FileChangeNotify>();
            })
            .Build();

        var loggerFactory = host.Services.GetService<ILoggerFactory>();
        var logger = loggerFactory!.CreateLogger(nameof(Program));

        try
        {
            await host.StartAsync().ConfigureAwait(true);
            await host.WaitForShutdownAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            logger.LogError("{Exception}", ex);
            throw;
        }
        finally
        {
            logger.LogInformation("Shutting down.");
        }
    }

    private static Setting GetSettings()
    {
        var settingsJson = JsonDocument.Parse(File.ReadAllText("appsettings.json"))
            .RootElement.GetProperty("settings").ToString();

        return JsonSerializer.Deserialize<Setting>(settingsJson) ??
            throw new ArgumentException("Could not deserialize appsettings into settings.");
    }

    public static Logger GetLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
    }
}
