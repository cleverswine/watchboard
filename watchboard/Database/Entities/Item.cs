using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WatchBoard.Services;

namespace WatchBoard.Database.Entities;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int TmdbId { get; set; }

    [Required]
    public ItemType Type { get; set; } = ItemType.Tv;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Notes { get; set; }

    [MaxLength(1024)]
    public string? Overview { get; set; }

    [MaxLength(255)]
    public string? TagLine { get; set; }

    public int RunTime { get; set; } = 0;

    // TV SHOW
    // ////////////////////////

    public SeriesStatus? SeriesStatus { get; set; }

    [MaxLength(50)]
    public string? SeriesNextEpisodeDate { get; set; }

    public int? SeriesNextEpisodeNumber { get; set; }
    public int? SeriesNextEpisodeSeason { get; set; }

    [MaxLength(80)]
    public string? EndDate { get; set; }

    public int NumberOfSeasons { get; set; }

    // COMMON
    // ////////////////////////

    [MaxLength(80)]
    public string? ReleaseDate { get; set; }

    [MaxLength(80)]
    public string? OriginalLanguage { get; set; }

    [MaxLength(80)]
    public string? OriginCountry { get; set; }

    [MaxLength(2048)]
    public string PosterUrl { get; set; } = string.Empty;

    [MaxLength(16384)]
    public string? PosterBase64 { get; set; }

    [MaxLength(2048)]
    public string BackdropUrl { get; set; } = string.Empty;

    [MaxLength(16384)]
    public string? BackdropBase64 { get; set; }
    
    public string? ProvidersJson { get; set; }

    [MaxLength(255)]
    public string? ImdbId { get; set; }

    public string? CreditsJson { get; set; }

    public DateTimeOffset? LastUpdated { get; set; } = DateTimeOffset.UtcNow;

    public Guid ListId { get; set; }
    public int Order { get; set; }

    public string ImdbUrl => $"https://www.imdb.com/title/{ImdbId}/";

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

    private ItemCredits? _credits;

    public ItemCredits GetCredits()
    {
        if (_credits != null) return _credits;
        if (CreditsJson == null) return new ItemCredits();
        var itemCredits = JsonSerializer.Deserialize<ItemCredits>(CreditsJson);
        if (itemCredits == null) return new ItemCredits();
        _credits = itemCredits;
        return _credits;
    }

    public void SetCredits(ItemCredits credits)
    {
        CreditsJson = JsonSerializer.Serialize(credits);
    }

    public string[] Crew(string job) => GetCredits().Crew.Where(x => x.Job != null && x.Job.Equals(job, StringComparison.OrdinalIgnoreCase)).Select(x => x.Name)
        .ToArray();

    public string[] Cast(int max = 6) => GetCredits().Cast.Take(max).Select(x => x.Name).ToArray();

    public string ReleaseDates()
    {
        if (ReleaseDate == null) return "";
        var dates = !string.IsNullOrWhiteSpace(EndDate) && ReleaseDate.Year() != EndDate.Year()
            ? $"{ReleaseDate.Year()} - {EndDate.Year()}"
            : $"{ReleaseDate.Year()}";
        return dates;
    }

    public string Seasons()
    {
        if (Type != ItemType.Tv) return "";
        var s = NumberOfSeasons > 1 ? "s" : "";
        return $"{NumberOfSeasons} season{s}";
    }
}