using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchBoard;
using WatchBoard.Database;
using WatchBoard.Database.Entities;
using WatchBoard.TmDb;
using WatchBoard.TmDb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
builder.Services.AddOpenApi();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ITmDbService, TmDbService>(opts =>
{
    opts.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", builder.Configuration["TmdbToken"]);
});
builder.Services.AddProblemDetails(o =>
{
    o.CustomizeProblemDetails = context =>
    {
        if (context.Exception is KeyNotFoundException)
        {
            context.HttpContext.Response.StatusCode = 404;
        }
    };
});

var app = builder.Build();

// Apply migrations and seed data if necessary
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.ApplyMigrations();
}

// OpenAPI/Swagger UI
app.MapOpenApi();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });

#region API

// Get Ordered Boards including Ordered Lists
app.MapGet("/boards", async (AppDbContext db) =>
    await db.Boards.OrderBy(x => x.Order)
        .Include(x => x.Lists.OrderBy(l => l.Order))
        .ToListAsync());

// Get Board including Ordered Lists
app.MapGet("/boards/{boardId:Guid}", async (AppDbContext db, Guid boardId) =>
    await db.Boards
        .Include(x => x.Lists.OrderBy(l => l.Order))
        .FirstOrDefaultAsync(x => x.Id == boardId)
    ?? throw new KeyNotFoundException("Board not found"));

// Get List including Ordered Items
app.MapGet("/lists/{listId:Guid}", async (AppDbContext db, Guid listId) =>
    await db.Lists
        .Include(x => x.Items.OrderBy(i => i.Order))
        .FirstOrDefaultAsync(x => x.Id == listId)
    ?? throw new KeyNotFoundException("List not found"));

// Get Item
app.MapGet("/items/{itemId:Guid}", async (AppDbContext db, Guid itemId) =>
{
    var item = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException("Item not found");
    var json = await db.Json.FirstOrDefaultAsync(x => x.ItemId == item.Id) ?? throw new KeyNotFoundException("Json not found");
    return item.ItemType == ItemType.TvShow
        ? Results.Ok(new {Item = item, TmDbItem = JsonSerializer.Deserialize<TvShow>(json.Data!)})
        : Results.Ok(new {Item = item, TmDbItem = JsonSerializer.Deserialize<Movie>(json.Data!)});
});

// Get Item by TmDbId
app.MapGet("/items/ByTmDbId/{tmDbId:int}", async (AppDbContext db, int tmDbId) =>
    await db.Items.FirstOrDefaultAsync(x => x.TmDbId == tmDbId) ?? throw new KeyNotFoundException("Item not found"));

// Get Poster
app.MapGet("/items/{itemId:Guid}/poster", async (AppDbContext db, Guid itemId) =>
{
    var i = await db.Items.Include(x => x.Poster).FirstOrDefaultAsync(x => x.Id == itemId)
            ?? throw new KeyNotFoundException("Item not found");
    return Results.File(i.Poster!.Data!, i.Poster.MediaType);
});

// Get Backdrop
app.MapGet("/items/{itemId:Guid}/backdrop", async (AppDbContext db, Guid itemId) =>
{
    var i = await db.Items.Include(x => x.Backdrop).FirstOrDefaultAsync(x => x.Id == itemId)
            ?? throw new KeyNotFoundException("Item not found");
    return Results.File(i.Backdrop!.Data!, i.Backdrop.MediaType);
});

// Add TV Show
app.MapPost("/boards/{boardId:guid}/tv/{tmDbId:int}", async (IRepository repository, Guid boardId, int tmDbId) =>
    await repository.AddOrUpdateItem(tmDbId, ItemType.TvShow, boardId));

// Add Movie
app.MapPost("/boards/{boardId:guid}/movie/{tmDbId:int}", async (IRepository repository, Guid boardId, int tmDbId) =>
    await repository.AddOrUpdateItem(tmDbId, ItemType.Movie, boardId));

// Refresh Item
app.MapPut("/items/{itemId:Guid}/refresh", async (AppDbContext db, IRepository repository, Guid itemId) =>
{
    var item = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException("Item not found");
    return await repository.AddOrUpdateItem(item.TmDbId, item.ItemType);
});

// Delete item
app.MapDelete("/items/{itemId:Guid}", async (AppDbContext db, Guid itemId) =>
{
    var item = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException("Item not found");
    db.Items.Remove(item);
    await db.SaveChangesAsync();
});

// Move Item
app.MapPut("/items/{itemId:Guid}/move",
    async (IRepository repository, Guid itemId, [FromQuery] Guid boardId) => { await repository.MoveItem(itemId, boardId); });

// Reorder List
app.MapPut("/lists/{listId:Guid}/reorder",
    async (IRepository repository, Guid listId, [FromBody] Guid[] newOrder) => { await repository.ReorderItems(listId, newOrder); });

app.MapGet("/search", async (ITmDbService tmDbService, [FromQuery] string q) =>
{
    var results = await tmDbService.Search(q);
    var result = results.Results.Select(async x =>
    {
        var posterUrl = await tmDbService.GetImageUrl(x.PosterPath ?? "", "w185");
        var backdropUrl = await tmDbService.GetImageUrl(x.BackdropPath ?? "", "w780");
        return new
        {
            x.Id, x.OriginalLanguage,
            Name = x.MediaType == "tv" ? x.Name : x.Title,
            ReleaseDate = x.MediaType == "tv" ? x.FirstAirDate : x.ReleaseDate,
            ItemType = x.MediaType == "tv" ? ItemType.TvShow : ItemType.Movie,
            PosterUrl = posterUrl,
            BackdropUrl = backdropUrl
        };
    });
    return Results.Ok(await Task.WhenAll(result));
});

#endregion

// app.UseExceptionHandler(exceptionHandlerApp 
//     => exceptionHandlerApp.Run(async context 
//         => await Results.Problem()
//             .ExecuteAsync(context)));

app.Run();