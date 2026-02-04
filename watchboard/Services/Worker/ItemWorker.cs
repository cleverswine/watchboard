using Microsoft.Extensions.Options;
using WatchBoard.Database.Entities;

namespace WatchBoard.Services.Worker;

public class WorkerConfig
{
    public int WorkerIntervalMinutes { get; set; } = 360; // 6 hours
    public int MinItemUpdateFrequencyMinutes { get; set; } = 60; // 1 hour
}

public class ItemWorker(IOptions<WorkerConfig> options, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            try
            {
                var dbItems = (await repository.GetItems()).ToList();

                await repository.AddSystemLog(new SystemLog {Type = SystemLogType.ItemWorkerInvoked, Message = $"Item worker invoked with {dbItems.Count} items to process"});

                foreach (var dbItem in dbItems)
                {
                    if (dbItem.LastUpdated != null &&
                        dbItem.LastUpdated > DateTimeOffset.UtcNow.AddMinutes(-options.Value.MinItemUpdateFrequencyMinutes)) continue;
                    await repository.RefreshItem(dbItem.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(options.Value.WorkerIntervalMinutes), stoppingToken);
        }
    }
}