using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbConfiguration
{
    [JsonPropertyName("images")]
    public TmDbConfigurationImages Images { get; set; } = new();
}