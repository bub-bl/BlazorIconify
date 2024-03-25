using System.Text.Json.Serialization;

namespace Iconify;

public record IconMetadata
{
    [JsonPropertyName("version")] public int Version { get; set; }
    [JsonPropertyName("content")] public string Content { get; set; }
    [JsonPropertyName("time_fetched")] public DateTime TimeFetched { get; set; }
}