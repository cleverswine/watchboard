using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbWatchProvidersResultsUs
{
    public string? Link { get; set; }

    [JsonPropertyName("flatrate")]
    public TmDbWatchProvidersResultsFlatRate[] FlatRate { get; set; } = [];
}