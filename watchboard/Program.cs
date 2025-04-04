using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using WatchBoard;
using WatchBoard.Services;
using WatchBoard.Services.Database;
using WatchBoard.Services.TmDb;
using WatchBoard.Services.Worker;

var builder = WebApplication.CreateBuilder();
builder.Services.AddRazorComponents();
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

// App Services
builder.Services.AddScoped<IRepository, Repository>();

var app = builder.Build();

// Apply migrations and seed data if necessary
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.ApplyMigrations();
}

app.UseStaticFiles();

// Routes
app.MapPageRoutes();
app.MapGroup("/app").MapPartials();

app.Run();