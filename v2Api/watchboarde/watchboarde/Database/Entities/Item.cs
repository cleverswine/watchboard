using WatchBoard.TmDb.Models;

namespace WatchBoard.Database.Entities;

public class Item : OrderedItem
{
    public Item()
    {
    }

    public Item(TvShow tmDb)
    {
        Update(tmDb);
    }

    public Item(Movie tmDb)
    {
        Update(tmDb);
    }

    public void Update(Movie tmDb)
    {
        Name = tmDb.Title;
        TmDbId = tmDb.Id;
        ItemType = ItemType.TvShow;
        if (tmDb.ReleaseDate != null)
            ReleaseDate = DateOnly.Parse(tmDb.ReleaseDate);
        Language = tmDb.OriginalLanguage;
    }
    
    public void Update(TvShow tmDb)
    {
        Name = tmDb.Name;
        TmDbId = tmDb.Id;
        ItemType = ItemType.TvShow;
        if (tmDb.FirstAirDate != null)
            ReleaseDate = DateOnly.Parse(tmDb.FirstAirDate);
        Language = tmDb.OriginalLanguage;
        if (tmDb.NextEpisodeToAir != null)
        {
            Notes = $"S{tmDb.NextEpisodeToAir.SeasonNumber} E{tmDb.NextEpisodeToAir.EpisodeNumber} -> {tmDb.NextEpisodeToAir.AirDate}";
        }
    }

    public int TmDbId { get; set; }
    public ItemType ItemType { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? Notes { get; set; }
    public string? Language { get; set; }
    public string? WatchProvider { get; set; }

    // public Guid BackdropId { get; set; } = Guid.Empty;
    public Backdrop Backdrop { get; set; }

    // public Guid PosterId { get; set; } = Guid.Empty;
    public Poster Poster { get; set; }

    // public Guid JsonId { get; set; } = Guid.Empty;
    public Json Json { get; set; }

    public Guid BoardListId { get; set; } = Guid.Empty;
}