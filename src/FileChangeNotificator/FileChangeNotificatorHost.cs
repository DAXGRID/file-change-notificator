using FileChangeNotificator.FileWatcher;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenFTTH.NotificationClient;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using NotificationClient = OpenFTTH.NotificationClient.Client;

namespace FileChangeNotificator;

internal sealed class FileChangeNotificatorHost : BackgroundService
{
    private readonly ILogger<FileChangeNotificatorHost> _logger;
    private readonly FileChangeNotify _fileChangeNotify;
    private readonly NotificationClient _notificationClient;

    public FileChangeNotificatorHost(
        ILogger<FileChangeNotificatorHost> logger,
        FileChangeNotify fileChangeNotify,
        Setting setting
    )
    {
        _logger = logger;
        _fileChangeNotify = fileChangeNotify;

        var ipAddress = Dns.GetHostEntry(setting.NotificationServerDomain).AddressList
            .First(x => x.AddressFamily == AddressFamily.InterNetwork);

        _notificationClient = new NotificationClient(
            ipAddress: ipAddress,
            port: setting.NotificationServerPort,
            writeOnly: true);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {Name}.", nameof(FileChangeNotificatorHost));
        _notificationClient.Connect();

        var f = (FileChangedEvent changedEvent) =>
        {
            _logger.LogInformation(
                "Changed: {FullPath} with {SHA256CheckSum}.",
                changedEvent.FullPath,
                changedEvent.Sha256CheckSum);

            _notificationClient.Send(
                new Notification(
                    "FileChangedEvent",
                    JsonSerializer.Serialize(changedEvent)
                ));
        };

        _fileChangeNotify.Start(f);

        // We create a file in the tmp folder to indicate that the service is healthy.
        using var _ = File.Create(Path.Combine(Path.GetTempPath(), "healthy"));
        _logger.LogInformation("{Service} is now healthy.", nameof(FileChangeNotificatorHost));

        return Task.CompletedTask;
    }
}
