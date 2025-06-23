namespace WatchBoard.Services.TmDb.Models;

[Serializable]
public class TmDbSearchResults
{
    public TmDbItem[] Results { get; set; } = [];
}