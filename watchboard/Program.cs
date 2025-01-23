using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using WatchBoard.Services;
using WatchBoard.Services.Database;
using WatchBoard.Services.TmDb;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "Static"
});
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

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
}

app.UseStaticFiles();

// Routes
app.MapGet("/", Routes.Home);

var appBase = app.MapGroup("/app");
// POST app/search
// GET app/boards
appBase.MapGet("/boards", Routes.Home);
// GET app/boards/1
// GET app/boards/1/lists
// GET app/boards/1/lists/2
// GET app/boards/1/lists/2/items
// PUT app/boards/1/lists/2/items (sort)
// GET app/boards/1/lists/2/items/3
// PUT app/boards/1/lists/2/items/3
// DELETE app/boards/1/lists/2/items/3
// PUT app/boards/1/lists/2/items/3/provider/amazon
// PUT app/boards/1/lists/2/items/3/backdrop/4
// POST app/boards/1/lists/2/items?TmDb=123&type=tv|movie

app.Run();