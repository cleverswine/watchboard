using System.ComponentModel.DataAnnotations;

namespace WatchBoard.Database.Entities;

public class List
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(255)]
    [Required]
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; } = 0;

    public List<Item> Items { get; set; } = [];
    public Guid BoardId { get; set; }
}