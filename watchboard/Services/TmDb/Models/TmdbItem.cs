using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmdbItem
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
    public Episode? NextEpisodeToAir { get; set; }
    
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
    public WatchProviders? Providers { get; set; } = new();
    [JsonPropertyName("external_ids")]
    public ExternalIds ExternalIds { get; set; } = new();

    public string? ItemName => Title ?? Name;
    public string? ItemReleaseDate => ReleaseDate ?? FirstAirDate;
}

[Serializable]
public class Episode
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;
    [JsonPropertyName("air_date")]
    public string AirDate { get; set; } = string.Empty;
    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }
    [JsonPropertyName("episode_type")]
    public string EpisodeType { get; set; } = string.Empty;
    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }
    [JsonPropertyName("show_id")]
    public int ShowId { get; set; }
    [JsonPropertyName("still_path")]
    public string StillPath { get; set; } = string.Empty;
}

