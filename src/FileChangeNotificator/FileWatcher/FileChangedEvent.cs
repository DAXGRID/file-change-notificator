using System.Text.Json.Serialization;

namespace FileChangeNotificator.FileWatcher;

public record FileChangedEvent
{
    [JsonPropertyName("eventId")]
    public Guid EventId { get; init; }

    [JsonPropertyName("eventType")]
    public string EventType { get; init; }

    [JsonPropertyName("eventTimeStamp")]
    public DateTime EventTimeStamp { get; init; }

    [JsonPropertyName("fullPath")]
    public string FullPath { get; init; }

    [JsonPropertyName("sha256CheckSum")]
    public string Sha256CheckSum { get; init; }

    [JsonConstructor]
    public FileChangedEvent(string fullPath, string sha256Checksum)
    {
        EventId = Guid.NewGuid();
        EventTimeStamp = DateTime.UtcNow;
        EventType = nameof(FileChangedEvent);
        FullPath = fullPath;
        Sha256CheckSum = sha256Checksum;
    }
}
