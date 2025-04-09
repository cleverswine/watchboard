using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbProviders
{
    [JsonPropertyName("results")]
    public List<TmDbProvider> Results { get; set; } = [];
}