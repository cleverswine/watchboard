using System.Text.Json.Serialization;

namespace WatchBoard.TmDb.Models;

[Serializable]
public class ImagesItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("backdrops")]
    public List<ImageItem> Backdrops { get; set; } = [];

    [JsonPropertyName("logos")]
    public List<ImageItem> Logos { get; set; } = [];

    [JsonPropertyName("posters")]
    public List<ImageItem> Posters { get; set; } = [];
}

[Serializable]
public class ImageItem
{
    [JsonPropertyName("aspect_ratio")]
    public double AspectRatio { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("iso_639_1")]
    public string? Iso639_1 { get; set; }

    [JsonPropertyName("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
}