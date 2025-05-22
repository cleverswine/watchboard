using System.Text.Json.Serialization;
using Humanizer;

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
    
    [JsonPropertyName("last_episode_to_air")]
    public TmDbItemEpisode? LastEpisodeToAir { get; set; }

    [JsonPropertyName("seasons")]
    public TmDbItemSeason[] Seasons { get; set; } = [];

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
    
    public string GetNotes()
    {
        if (NextEpisodeToAir == null)
        {
            if (LastEpisodeToAir == null) return "";
            var then = DateOnly.Parse(LastEpisodeToAir.AirDate);
            return $"S{LastEpisodeToAir.SeasonNumber} finale -> {HumanizeWithTodayOption(then)}";
        }
        else
        {
            var then = DateOnly.Parse(NextEpisodeToAir.AirDate);
            return
                $"S{NextEpisodeToAir.SeasonNumber} E{NextEpisodeToAir.EpisodeNumber}/{EpisodeCount(NextEpisodeToAir.SeasonNumber)} -> {HumanizeWithTodayOption(then)}";
        }

        string HumanizeWithTodayOption(DateOnly d)
        {
            var result = d.Humanize();
            return result == "now" ? "today!" : result;
        }

        string EpisodeCount(int seasonNumber)
        {
            var season = Seasons.FirstOrDefault(x => x.SeasonNumber == seasonNumber);
            return season?.EpisodeCount.ToString() ?? "?";
        }
    }
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

public class TmDbItemSeason
{
    [JsonPropertyName("air_date")]
    public string AirDate { get; set; } = string.Empty;

    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }
}