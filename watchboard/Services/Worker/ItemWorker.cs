using Microsoft.EntityFrameworkCore;
using WatchBoard.Database;
using WatchBoard.Services.Database;
using WatchBoard.Services.TmDb;

namespace WatchBoard.Services.Worker;

// TODO - make configurable
public class WorkerConfig
{
    public int WorkerIntervalMinutes { get; set; } = 360; // 6 hours
    public int MinItemUpdateFrequencyMinutes { get; set; } = 120; // 2 hours
}

public class ItemWorker(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly WorkerConfig _workerConfig = new();
    
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
                var dbItems = await db.Items.ToListAsync(stoppingToken);
                foreach (var dbItem in dbItems)
                {
                    if (dbItem.LastUpdated != null && dbItem.LastUpdated > DateTimeOffset.UtcNow.AddMinutes(-_workerConfig.MinItemUpdateFrequencyMinutes)) continue;

                    var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                    var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                    dbItem.UpdateFromTmDb(tmDbItem, images);
                    dbItem.LastUpdated = DateTimeOffset.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(_workerConfig.WorkerIntervalMinutes), stoppingToken);
        }
    }
}