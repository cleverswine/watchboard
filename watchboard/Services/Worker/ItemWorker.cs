using Microsoft.EntityFrameworkCore;
using WatchBoard.Services.Database;
using WatchBoard.Services.TmDb;

namespace WatchBoard.Services.Worker;

public class ItemWorker(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var tmDb = scope.ServiceProvider.GetRequiredService<ITmDb>();

            try
            {
                var dbItems = await db.Items.ToListAsync(cancellationToken: stoppingToken);
                foreach (var dbItem in dbItems)
                {
                    if (dbItem.LastUpdated != null && dbItem.LastUpdated > DateTimeOffset.UtcNow.AddHours(-4)) continue;

                    var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                    var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                    dbItem.MapFrom(tmDbItem, images);
                    dbItem.LastUpdated = DateTimeOffset.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
}