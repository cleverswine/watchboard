using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using WatchBoard;
using WatchBoard.Services.Database;
using WatchBoard.Services.TmDb;
using WatchBoard.Services.Worker;

var builder = WebApplication.CreateBuilder();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();

// Worker to update items every x hours
if (!builder.Environment.IsDevelopment())
    builder.Services.AddHostedService<ItemWorker>();

// User Config
var dataPath = Environment.GetEnvironmentVariable("DATA_DIR") ?? "/data";
if (!Path.Exists(dataPath)) Directory.CreateDirectory(dataPath);

var userConfig = Path.Combine(dataPath, "appsettings.json");
builder.Configuration.AddJsonFile(userConfig, true);

// Database
var dataSource = Path.Combine(dataPath, "watchboard.db");
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlite($"Data Source={dataSource}"));

// TmDb
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ITmDb, TmDb>(opts =>
{
    opts.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration["TmdbToken"]);
});

var app = builder.Build();

// Apply migrations and seed data if necessary
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.ApplyMigrations();

    // var tmDb = scope.ServiceProvider.GetRequiredService<ITmDb>();
    // int[] ids = [125988,1411,79340,47141,117488,95396];
    // var board = context.Boards.First(x => x.Name == "Kevin");
    // var list = context.Lists.First(x => x.BoardId == board.Id);
    // foreach (var id in ids)
    // {
    //     var tmDbItem = await tmDb.GetDetail(id, "tv");
    //     var images = await tmDb.GetImages(id, "tv");
    //     var dbItem = tmDbItem.MapTo(list.Id, images);
    //     dbItem.BackdropBase64 = await tmDb.GetImageBase64(dbItem.BackdropUrl, "w300");
    //     dbItem.PosterBase64 = await tmDb.GetImageBase64(dbItem.PosterUrl, "w92");
    //     context.Items.Add(dbItem);
    // }
    // await context.SaveChangesAsync();
}

app.UseStaticFiles();

// Routes
app.MapPages();
app.MapGroup("/app").MapHomePartials();
app.MapGroup("/admin").MapAdminPartials();

app.Run();