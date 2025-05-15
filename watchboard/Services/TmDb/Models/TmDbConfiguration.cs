using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbConfiguration
{
    [JsonPropertyName("images")]
    public TmDbConfigurationImages Images { get; set; } = new();

    public List<TmDbConfigurationCountry> Countries { get; set; } = [];
    public List<TmDbConfigurationLanguage> Languages { get; set; } = [];
}

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

[Serializable]
public class TmDbConfigurationImages
{
    [JsonPropertyName("base_url")]
    public string? BaseUrl { get; set; }

    [JsonPropertyName("secure_base_url")]
    public string? SecureBaseUrl { get; set; }

    [JsonPropertyName("backdrop_sizes")]
    public string[] BackdropSizes { get; set; } = [];

    [JsonPropertyName("logo_sizes")]
    public string[] LogoSizes { get; set; } = [];

    [JsonPropertyName("poster_sizes")]
    public string[] PosterSizes { get; set; } = [];

    [JsonPropertyName("profile_sizes")]
    public string[] ProfileSizes { get; set; } = [];

    [JsonPropertyName("still_sizes")]
    public string[] StillSizes { get; set; } = [];
}

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