using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileChangeNotificator;

internal sealed class FileChangeNotificatorHost : BackgroundService
{
    private ILogger<FileChangeNotificatorHost> _logger;

    public FileChangeNotificatorHost(ILogger<FileChangeNotificatorHost> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {Name}.", nameof(FileChangeNotificatorHost));

        // We create a file in the tmp folder to indicate that the service is healthy.
        using var _ = File.Create(Path.Combine(Path.GetTempPath(), "healthy"));
        _logger.LogInformation("{Service} is now healthy.", nameof (FileChangeNotificatorHost));

        return Task.CompletedTask;
    }
}
