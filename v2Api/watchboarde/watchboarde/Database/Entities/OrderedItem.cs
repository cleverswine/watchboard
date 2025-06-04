namespace WatchBoard.Database.Entities;

public abstract class OrderedItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public int Order { get; set; }
}