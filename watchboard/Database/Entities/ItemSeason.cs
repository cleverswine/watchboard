namespace WatchBoard.Database.Entities;

[Serializable]
public class ItemSeason
{
    public string AirDate { get; set; } = string.Empty;
    public int EpisodeCount { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string PosterPath { get; set; } = string.Empty;
    public int SeasonNumber { get; set; }
    public double VoteAverage { get; set; }

    public List<ItemEpisode> Episodes { get; set; } = [];
}