using System.Text.Json.Serialization;

namespace FileChangeNotificator;

internal sealed record Setting
{
    [JsonPropertyName("watchDirectory")]
    public string WatchDirectory { get; init; }

    [JsonPropertyName("notificationServerDomain")]
    public string NotificationServerDomain { get; init; }

    [JsonPropertyName("notificationServerPort")]
    public int NotificationServerPort { get; init; }

    [JsonConstructor]
    public Setting(
        string watchDirectory,
        string notificationServerDomain,
        int notificationServerPort)
    {
        if (string.IsNullOrWhiteSpace(watchDirectory))
        {
            throw new ArgumentException(
                $"'{nameof(watchDirectory)}' cannot be null or whitespace.",
                nameof(watchDirectory));
        }

        if (string.IsNullOrWhiteSpace(notificationServerDomain))
        {
            throw new ArgumentException(
                $"'{nameof(notificationServerDomain)}' cannot be null or whitespace.",
                nameof(notificationServerDomain));
        }

        WatchDirectory = watchDirectory;
        NotificationServerDomain = notificationServerDomain;
        NotificationServerPort = notificationServerPort;
    }
}
