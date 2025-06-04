using System.Text.Json.Serialization;

namespace WatchBoard.TmDb.Models;

public abstract class TmDbItemBase
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }
    
    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }
}