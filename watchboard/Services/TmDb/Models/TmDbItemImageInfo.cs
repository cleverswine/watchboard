using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbItemImageInfo
{
    [JsonPropertyName("aspect_ratio")]
    public double AspectRatio { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("iso_639_1")]
    public string? Iso6391 { get; set; }

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
}