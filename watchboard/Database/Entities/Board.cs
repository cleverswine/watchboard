using System.ComponentModel.DataAnnotations;

namespace WatchBoard.Database.Entities;

public class Board
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public int? Order { get; set; }

    public List<List> Lists { get; set; } = [];
}