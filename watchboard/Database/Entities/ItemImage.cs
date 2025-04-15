namespace WatchBoard.Database.Entities;

public class ItemImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ItemImageType Type { get; set; } = ItemImageType.Backdrop;
    public string Name { get; set; } = string.Empty;
    public string UrlPath { get; set; } = string.Empty;
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
}