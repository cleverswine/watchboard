namespace WatchBoard.Database.Entities;

public class BoardList : OrderedItem
{
    public bool Default { get; set; }
    public Guid BoardId { get; set; }
    public List<Item> Items { get; set; } = [];
}