using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WatchBoard.Services.Database.Entities;

public enum ItemType
{
    Tv,
    Movie
}

public enum SeriesStatus
{
    InProgress,
    Returning,
    Ended
}

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public ItemType Type { get; set; } = ItemType.Tv;

    public SeriesStatus? SeriesStatus { get; set; }

    [MaxLength(50)]
    public string? SeriesNextEpisodeDate { get; set; }

    public int? SeriesNextEpisodeNumber { get; set; }
    public int? SeriesNextEpisodeSeason { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string? Overview { get; set; }

    [MaxLength(255)]
    public string? TagLine { get; set; }

    [MaxLength(80)]
    public string? ReleaseDate { get; set; }

    [MaxLength(80)]
    public string? EndDate { get; set; }

    public int NumberOfSeasons { get; set; }

    public int Order { get; set; } = 0;

    [MaxLength(2048)]
    public string BackdropUrl { get; set; } = string.Empty;

    [MaxLength(16384)]
    public string? BackdropBase64 { get; set; }

    [MaxLength(2048)]
    public string PosterUrl { get; set; } = string.Empty;

    [MaxLength(16384)]
    public string? PosterBase64 { get; set; }

    [MaxLength(2048)]
    public string? ProviderNamesCsv { get; set; }

    [MaxLength(255)]
    public string? SelectedProviderName { get; set; }

    [MaxLength(255)]
    public string? ImdbId { get; set; }

    [MaxLength(255)]
    public int TmdbId { get; set; }

    public string? Images { get; set; }

    public DateTimeOffset? LastUpdated { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public bool Expanded { get; set; } = false;

    public Guid ListId { get; set; }

    public List<string> ProviderOptions => ProviderNamesCsv?.Split(",").ToList() ?? [];

    public string ImdbUrl => $"https://www.imdb.com/title/{ImdbId}/";

    public string TmdbUrl => $"https://www.themoviedb.org/{Type.ToString().ToLower()}/{TmdbId}";

    public List<ItemImage> GetBackdropImages()
    {
        return Images == null ? [] : JsonSerializer.Deserialize<List<ItemImage>>(Images)?
            .Where(x => x.Type == ItemImageType.Backdrop).ToList() ?? [];
    }

    public void SetImages(List<ItemImage> images)
    {
        Images = JsonSerializer.Serialize(images);
    }

    public string ReleaseDates()
    {
        if (ReleaseDate == null) return "";
        var dates = !string.IsNullOrWhiteSpace(EndDate)
            ? $"{Year(ReleaseDate)} - {Year(EndDate)}"
            : $"{Year(ReleaseDate)}";
        if (this is {Type: ItemType.Tv, NumberOfSeasons: > 0}) dates += $" ({NumberOfSeasons} seasons)";
        return dates;
    }

    private string Year(string s)
    {
        return string.IsNullOrWhiteSpace(s) ? "" : s.Split('-').First();
    }
}

public enum ItemImageType
{
    Backdrop,
    Logo,
    Poster
}

public class ItemImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ItemImageType Type { get; set; } = ItemImageType.Backdrop;
    public string Name { get; set; } = string.Empty;
    public string UrlPath { get; set; } = string.Empty;
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
}