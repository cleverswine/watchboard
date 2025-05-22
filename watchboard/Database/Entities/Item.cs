using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace WatchBoard.Database.Entities;

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

    [MaxLength(255)]
    public string? Notes { get; set; }

    [MaxLength(1024)]
    public string? Overview { get; set; }

    [MaxLength(255)]
    public string? TagLine { get; set; }

    [MaxLength(80)]
    public string? ReleaseDate { get; set; }

    [MaxLength(80)]
    public string? EndDate { get; set; }

    public int NumberOfSeasons { get; set; }

    [MaxLength(80)]
    public string? OriginalLanguage { get; set; }

    [MaxLength(80)]
    public string? OriginCountry { get; set; }
    
    public int Order { get; set; }

    [MaxLength(2048)]
    public string BackdropUrl { get; set; } = string.Empty;

    [MaxLength(16384)]
    public string? BackdropBase64 { get; set; }

    [MaxLength(2048)]
    public string PosterUrl { get; set; } = string.Empty;

    [MaxLength(16384)]
    public string? PosterBase64 { get; set; }

    public string? ProvidersJson { get; set; }

    [MaxLength(255)]
    public string? ImdbId { get; set; }

    [MaxLength(255)]
    public int TmdbId { get; set; }

    public string? Images { get; set; }

    public DateTimeOffset? LastUpdated { get; set; } = DateTimeOffset.UtcNow;

    public Guid ListId { get; set; }

    public string ImdbUrl => $"https://www.imdb.com/title/{ImdbId}/";

    public string TmdbUrl => $"https://www.themoviedb.org/{Type.ToString().ToLower()}/{TmdbId}";

    public List<ItemImage> GetBackdropImages()
    {
        return Images == null
            ? []
            : JsonSerializer.Deserialize<List<ItemImage>>(Images)?
                .Where(x => x.Type == ItemImageType.Backdrop).ToList() ?? [];
    }

    public void SetImages(List<ItemImage> images)
    {
        Images = JsonSerializer.Serialize(images);
    }

    public List<ItemProvider> GetProviders()
    {
        if (ProvidersJson == null) return [];
        var itemProviders = JsonSerializer.Deserialize<List<ItemProvider>>(ProvidersJson) ?? throw new JsonException("Could not deserialize ProvidersJson");
        return itemProviders;
    }

    public void SetProviders(List<ItemProvider> providers)
    {
        ProvidersJson = JsonSerializer.Serialize(providers);
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