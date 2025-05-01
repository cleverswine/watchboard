using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbConfigurationLanguage
{
    [JsonPropertyName("iso_639_1")]
    public string? Iso6391 { get; set; }

    [JsonPropertyName("english_name")]
    public string? EnglishName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}