using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchBoard.Database.Entities;

public enum SystemLogType
{
    ApplicationStarted,
    ItemWorkerInvoked,
    ItemRefreshed,
    LogsPruned
}

public class SystemLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? WorkerId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    [MaxLength(1024)]
    public string Message { get; set; } = string.Empty;
    public SystemLogType Type { get; set; }
    public Guid? ItemId { get; set; }

    [ForeignKey("ItemId")]
    public Item? Item { get; set; }
}