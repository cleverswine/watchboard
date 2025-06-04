namespace WatchBoard.Database.Entities;

public abstract class Image
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public byte[]? Data { get; set; }
    public string? MediaType { get; set; }
    
    public Guid ItemId { get; set; }
}