using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbProviders
{
    [JsonPropertyName("results")]
    public List<TmDbProvider> Results { get; set; } = [];
}

[Serializable]
public class TmDbProvider
{
    [JsonPropertyName("display_priorities")]
    public Dictionary<string, int> DisplayPriorities { get; set; } = [];

    [JsonPropertyName("display_priority")]
    public int DisplayPriority { get; set; }

    [JsonPropertyName("logo_path")]
    public string LogoPath { get; set; } = string.Empty;

    [JsonPropertyName("provider_name")]
    public string ProviderName { get; set; } = string.Empty;

    [JsonPropertyName("provider_id")]
    public int ProviderId { get; set; }
}