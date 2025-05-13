using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbItemSeason
{
    [JsonPropertyName("_id")]
    public int Id { get; set; }

    [JsonPropertyName("episodes")]
    public List<TmDbItemSeasonEpisode> Episodes { get; set; } = [];
}