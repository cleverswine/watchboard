using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbItemImageList
{
    [JsonPropertyName("backdrops")]
    public TmDbItemImageInfo[] Backdrops { get; set; } = [];

    [JsonPropertyName("logos")]
    public TmDbItemImageInfo[] Logos { get; set; } = [];

    [JsonPropertyName("posters")]
    public TmDbItemImageInfo[] Posters { get; set; } = [];
}