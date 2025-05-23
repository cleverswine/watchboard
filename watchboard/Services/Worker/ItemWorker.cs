namespace WatchBoard.Services.Worker;

// TODO - make configurable
public class WorkerConfig
{
    public int WorkerIntervalMinutes { get; set; } = 180; // 3 hours
    public int MinItemUpdateFrequencyMinutes { get; set; } = 60; // 1 hour
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
            var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            try
            {
                var dbItems = await repository.GetItems();
                foreach (var dbItem in dbItems)
                {
                    if (dbItem.LastUpdated != null &&
                        dbItem.LastUpdated > DateTimeOffset.UtcNow.AddMinutes(-_workerConfig.MinItemUpdateFrequencyMinutes)) continue;
                    await repository.RefreshItem(dbItem.Id);
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