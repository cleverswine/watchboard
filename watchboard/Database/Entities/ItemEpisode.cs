namespace WatchBoard.Database.Entities;

[Serializable]
public class ItemEpisode
{
    public int TmDbItemId { get; set; }
    public string AirDate { get; set; } = string.Empty;
    public int EpisodeNumber { get; set; }
    public string EpisodeType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string ProductionCode { get; set; } = string.Empty;
    public int? Runtime { get; set; }
    public int SeasonNumber { get; set; }
    public string StillPath { get; set; } = string.Empty;
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
}