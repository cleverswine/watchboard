using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbSeason
{
    [JsonPropertyName("air_date")]
    public string AirDate { get; set; } = string.Empty;

    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }
    
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("episodes")]
    public List<TmDbEpisode> Episodes { get; set; } = [];
}

[Serializable]
public class TmDbEpisode
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("show_id")]
    public int TmDbItemId { get; set; }

    [JsonPropertyName("air_date")]
    public string AirDate { get; set; } = string.Empty;

    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("episode_type")]
    public string EpisodeType { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("production_code")]
    public string ProductionCode { get; set; } = string.Empty;

    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("still_path")]
    public string StillPath { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
}