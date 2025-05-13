using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbItem
{
    public int Id { get; set; }

    // Movie
    public string? Title { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    // TV
    public string? Name { get; set; }

    [JsonPropertyName("number_of_seasons")]
    public int NumberOfSeasons { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; }

    [JsonPropertyName("last_air_date")]
    public string? LastAirDate { get; set; }

    [JsonPropertyName("next_episode_to_air")]
    public TmDbItemEpisode? NextEpisodeToAir { get; set; }

    // Common
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    public string? Overview { get; set; }

    [JsonPropertyName("tagline")]
    public string? TagLine { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }

    [JsonPropertyName("watch/providers")]
    public TmDbWatchProviders? Providers { get; set; } = new();

    [JsonPropertyName("external_ids")]
    public TmDbItemExternalIds ExternalIds { get; set; } = new();

    [JsonPropertyName("origin_country")]
    public string[] OriginCountry { get; set; } = [];

    [JsonPropertyName("original_language")]
    public string? OriginalLanguage { get; set; }

    public string? ItemName => Title ?? Name;
    public string? ItemReleaseDate => ReleaseDate ?? FirstAirDate;
}