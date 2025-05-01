using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbConfigurationCountry
{
    [JsonPropertyName("iso_3166_1")]
    public string? Iso31661 { get; set; }

    [JsonPropertyName("english_name")]
    public string? EnglishName { get; set; }

    [JsonPropertyName("native_name")]
    public string? NativeName { get; set; }
}