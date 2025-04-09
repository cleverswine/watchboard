using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbWatchProvidersResults
{
    [JsonPropertyName("US")]
    public TmDbWatchProvidersResultsUs Us { get; set; } = new();
}