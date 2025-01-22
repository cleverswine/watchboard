using System.Text.Json.Serialization;

namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class Results
{
    [JsonPropertyName("US")]
    public Us Us { get; set; } = new();
}