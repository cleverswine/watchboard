using Microsoft.EntityFrameworkCore;
using WatchBoard.Database.Entities;

namespace WatchBoard.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardList> Lists { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Poster> Posters { get; set; }
    public DbSet<Backdrop> Backdrops { get; set; }
    public DbSet<Json> Json { get; set; }

    public async Task ApplyMigrations()
    {
        await Database.MigrateAsync();

        if (!Boards.Any())
        {
            await AddBoard(new Board
            {
                Name = "Kevin's TV Shows",
                Order = 0,
                Lists =
                [
                    new BoardList
                    {
                        Name = "Watching",
                        Order = 0
                    },
                    new BoardList
                    {
                        Name = "Queue",
                        Order = 1,
                        Default = true
                    }
                ]
            });
        }
    }

    private async Task AddBoard(Board board)
    {
        await Boards.AddAsync(board);
        await SaveChangesAsync();
    }
}