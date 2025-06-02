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
                    new List {Name = "Future", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });
            Boards.Add(new Board
            {
                Name = "My Movies",
                Order = 1,
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
                Order = 2,
                Lists =
                [
                    new List {Name = "Future", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });
            Boards.Add(new Board
            {
                Name = "Family Movies",
                Order = 3,
                Lists =
                [
                    new List {Name = "Future", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });
            await SaveChangesAsync();
        }

        var b = await Boards.FirstOrDefaultAsync(x => x.Name == "Movies");
        if (b != null)
        {
            b.Name = "My Movies";
            await SaveChangesAsync();
        }
    }
}