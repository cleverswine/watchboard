namespace WatchBoard.Database.Entities;

[Serializable]
public class ItemProvider
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? DisplayPriority { get; set; }
    public bool Selected { get; set; }
}