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

        if (Boards.FirstOrDefault(x => x.Name == "Movies") == null)
        {
            Boards.Add(new Board
            {
                Name = "Movies",
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
                Name = "Family Movies",
                Order = 3,
                Lists =
                [
                    new List {Name = "Future", Order = 0, Items = []},
                    new List {Name = "Queue", Order = 1, Items = []},
                    new List {Name = "Watching", Order = 2, Items = []}
                ]
            });

            var b0 = Boards.FirstOrDefault(x => x.Name == "My Stuff");
            if (b0 != null) b0.Order = 0;
            var b2 = Boards.FirstOrDefault(x => x.Name == "Family");
            if (b2 != null) b2.Order = 2;
            
            await SaveChangesAsync();
        }
    }
}