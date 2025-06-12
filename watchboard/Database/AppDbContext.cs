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
                Order = 0,
                Lists =
                [
                    new List {Name = "Started", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Default = true, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });
            Boards.Add(new Board
            {
                Name = "Family Stuff",
                Order = 1,
                Lists =
                [
                    new List {Name = "Started", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Default = true, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });
            await SaveChangesAsync();
        }

        var lists = Lists.Where(x => x.Name == "Queue");
        foreach (var list in lists)
        {
            list.Default = true;
        }
        await SaveChangesAsync();
    }
}