using Microsoft.Extensions.Logging;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Security.Cryptography;

namespace FileChangeNotificator.FileWatcher;

internal sealed class FileChangeNotify : IDisposable
{
    private readonly ILogger<FileChangeNotify> _logger;
    private readonly FileSystemWatcher _watcher;
    private readonly Setting _watchSetting;
    private Action<FileChangedEvent>? _fileChangedCallback;

    public FileChangeNotify(
        ILogger<FileChangeNotify> logger,
        Setting setting)
    {
        _logger = logger;
        _watchSetting = setting;
        _watcher = new FileSystemWatcher(setting.WatchDirectory);
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }

    public void Start(Action<FileChangedEvent> fileChangeCallback)
    {
        _fileChangedCallback = fileChangeCallback;

        _watcher.IncludeSubdirectories = true;
        _watcher.NotifyFilter = NotifyFilters.LastWrite;
        _watcher.Error += OnError;
        _watcher.EnableRaisingEvents = true;

        Observable.FromEventPattern<FileSystemEventArgs>(_watcher, "Changed")
               .GroupByUntil(
                    item => item.EventArgs.FullPath,
                    item => Observable.Timer(TimeSpan.FromSeconds(10), Scheduler.Default))
            .SelectMany(y => y.LastAsync())
            .Subscribe(OnChanged);
    }

    private void OnChanged(EventPattern<FileSystemEventArgs> obj)
    {
        var (_, e) = obj;
        if (e == null)
        {
            return;
        }

        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }

        var sha256CheckSum = SHA256CheckSum(e.FullPath);
        if (string.IsNullOrWhiteSpace(sha256CheckSum))
        {
            throw new InvalidOperationException(
                $"File '{e.FullPath}' SHA256Checksum cannot be null, empty or empty.");
        }

        // We replace it here so only from the current path from fileserver.
        var fullPath = e.FullPath.Replace(
            _watchSetting.WatchDirectory,
            string.Empty,
            StringComparison.Ordinal);

        _fileChangedCallback!(new(fullPath, sha256CheckSum));
    }

    private void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private void PrintException(Exception? ex)
    {
        if (ex is not null)
            _logger.LogError("{Error}", ex);
    }

    private static string SHA256CheckSum(string filePath)
    {
        using var sha256 = SHA256.Create();
        using FileStream fileStream = File.OpenRead(filePath);
        return Convert.ToBase64String(sha256.ComputeHash(fileStream));
    }
}
