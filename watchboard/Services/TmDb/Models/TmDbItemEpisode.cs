using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

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