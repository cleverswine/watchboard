using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmdbConfiguration
{
    public Images Images { get; set; } = new();
}

[Serializable]
public class Images
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