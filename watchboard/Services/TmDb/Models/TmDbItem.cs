using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    // Movie
    public string? Title { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    // TV
    [JsonPropertyName("name")]
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

    [JsonPropertyName("overview")]
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

[Serializable]
public class TmDbItemEpisode
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

[Serializable]
public class TmDbItemExternalIds
{
    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("tvdb_id")]
    public int? TvDbId { get; set; }
}

[Serializable]
public class TmDbWatchProviders
{
    public TmDbWatchProvidersResults Results { get; set; } = new();
}

[Serializable]
public class TmDbWatchProvidersResults
{
    [JsonPropertyName("US")]
    public TmDbWatchProvidersResultsUs Us { get; set; } = new();
}


[Serializable]
public class TmDbWatchProvidersResultsUs
{
    public string? Link { get; set; }

    [JsonPropertyName("flatrate")]
    public TmDbWatchProvidersResultsFlatRate[] FlatRate { get; set; } = [];
}

[Serializable]
public class TmDbWatchProvidersResultsFlatRate
{
    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; set; }

    [JsonPropertyName("provider_id")]
    public int ProviderId { get; set; }

    [JsonPropertyName("provider_name")]
    public string? ProviderName { get; set; }

    [JsonPropertyName("display_priority")]
    public int DisplayPriority { get; set; }
    
    [JsonIgnore]
    public bool Selected { get; set; }
}