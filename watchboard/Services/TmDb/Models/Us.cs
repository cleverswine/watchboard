using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class Us
{
    public string? Link { get; set; }

    [JsonPropertyName("flatrate")]
    public FlatRate[] FlatRate { get; set; } = [];
}