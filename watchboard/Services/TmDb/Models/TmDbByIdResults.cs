using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbByIdResults
{
    [JsonPropertyName("movie_results")]
    public TmDbItem[] MovieResults { get; set; } = [];

    [JsonPropertyName("tv_results")]
    public TmDbItem[] TvResults { get; set; } = [];
}