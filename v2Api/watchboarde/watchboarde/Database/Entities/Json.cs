namespace WatchBoard.Database.Entities;

public class Json
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Data { get; set; }
    public string? DataType { get; set; }
    
    public Guid ItemId { get; set; }
}