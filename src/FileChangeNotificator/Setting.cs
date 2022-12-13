using System.Text.Json.Serialization;

namespace FileChangeNotificator;

internal sealed record Setting
{
    [JsonPropertyName("watchDirectory")]
    public string WatchDirectory { get; init; }

    [JsonPropertyName("notificationServerUri")]
    public string NotificationServerUri { get; init; }

    [JsonConstructor]
    public Setting(string watchDirectory, string notificationServerUri)
    {
        if (string.IsNullOrWhiteSpace(watchDirectory))
        {
            throw new ArgumentException(
                $"'{nameof(watchDirectory)}' cannot be null or whitespace.",
                nameof(watchDirectory));
        }

        if (string.IsNullOrWhiteSpace(notificationServerUri))
        {
            throw new ArgumentException(
                $"'{nameof(notificationServerUri)}' cannot be null or whitespace.",
                nameof(notificationServerUri));
        }

        WatchDirectory = watchDirectory;
        NotificationServerUri = notificationServerUri;
    }
}
