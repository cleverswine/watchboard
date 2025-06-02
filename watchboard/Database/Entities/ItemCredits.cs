namespace WatchBoard.Database.Entities;

[Serializable]
public class ItemCredits
{
    public List<ItemCreditCastMember> Cast { get; set; } = [];
    public List<ItemCreditCrewMember> Crew { get; set; } = [];
}

[Serializable]
public class ItemCreditCastMember
{
    public required string Name { get; set; }
    public string? ProfilePath { get; set; }
    public required string Character { get; set; }
    public int Order { get; set; }
}

[Serializable]
public class ItemCreditCrewMember
{
    public required string Name { get; set; }
    public string? ProfilePath { get; set; }
    public string? Department { get; set; }
    public string? Job { get; set; }
}