using Microsoft.EntityFrameworkCore;
using WatchBoard.Database.Entities;

namespace WatchBoard.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<Item> Items { get; set; }

    public async Task ApplyMigrations()
    {
        await Database.MigrateAsync();

        if (!Boards.Any())
        {
            Boards.Add(new Board
            {
                Name = "My Stuff",
                Lists =
                [
                    new List {Name = "Future", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });
            Boards.Add(new Board
            {
                Name = "Family",
                Lists =
                [
                    new List {Name = "Future", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });
            await SaveChangesAsync();
        }
    }
}