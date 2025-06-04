namespace WatchBoard.Database.Entities;

public class Board : OrderedItem
{
    public List<BoardList> Lists { get; set; } = [];
}