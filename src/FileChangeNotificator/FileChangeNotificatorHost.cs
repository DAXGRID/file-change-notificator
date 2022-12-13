using FileChangeNotificator.FileWatcher;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileChangeNotificator;

internal sealed class FileChangeNotificatorHost : BackgroundService
{
    private readonly ILogger<FileChangeNotificatorHost> _logger;
    private readonly FileChangeNotify _fileChangeNotify;
    private readonly Setting _settings;

    public FileChangeNotificatorHost(
        ILogger<FileChangeNotificatorHost> logger,
        FileChangeNotify fileChangeNotify,
        Setting setting
    )
    {
        _logger = logger;
        _fileChangeNotify = fileChangeNotify;
        _settings = setting;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {Name}.", nameof(FileChangeNotificatorHost));

        var f = (FileChangedEvent changedEvent) =>
        {
            _logger.LogInformation(
                "Changed: {FullPath} with {SHA256CheckSum}.",
                changedEvent.FullPath,
                changedEvent.Sha256CheckSum);
        };

        _fileChangeNotify.Start(f);

        // We create a file in the tmp folder to indicate that the service is healthy.
        using var _ = File.Create(Path.Combine(Path.GetTempPath(), "healthy"));
        _logger.LogInformation("{Service} is now healthy.", nameof(FileChangeNotificatorHost));

        return Task.CompletedTask;
    }
}
