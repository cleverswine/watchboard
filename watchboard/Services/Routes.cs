using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchBoard.Pages;
using WatchBoard.Services.Database;
using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.TmDb;

namespace WatchBoard.Services;

public static class Routes
{
    // GET /
    public static async Task<IResult> Home()
    {
        await Task.Yield();
        return new RazorComponentResult<Home>(new {Message = "Hello World!"});
    }
    
    // POST app/search
    public static async Task<IResult> DoSearch(HttpRequest request, [FromServices] ITmDb tmDb)
    {
        var form = await request.ReadFormAsync();
        var s = form["SearchName"];
        
        var tmDbResults = await tmDb.Search(s!);
        var items = tmDbResults.Select(x => new Item
        {
            Name = x.ItemName ?? "UNKNOWN",
            Type = x.MediaType == "tv" ? ItemType.Tv : ItemType.Movie,
            TagLine = x.TagLine,
            ReleaseDate = x.ItemReleaseDate,
            EndDate = x.LastAirDate,
            NumberOfSeasons = x.NumberOfSeasons,
            TmdbId = x.Id,
            PosterUrl = x.PosterPath ?? "UNKNOWN",
        }).ToList();
        
        return new RazorComponentResult<Home>(new {Message = "Hello World!"}); // _SearchResults
    }
    
    // GET app/boards
    public static async Task<IResult> GetBoards()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // GET app/boards/1
    public static async Task<IResult> GetBoard()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // GET app/boards/1/lists
    public static async Task<IResult> GetLists()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // GET app/boards/1/lists/2
    public static async Task<IResult> GetList()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // GET app/boards/1/lists/2/items
    public static async Task<IResult> GetItems()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // GET app/boards/1/lists/2/items/3
    public static async Task<IResult> GetItem()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // DELETE app/boards/1/lists/2/items/3
    public static async Task<IResult> DeleteItem()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // PUT app/boards/1/lists/2/items/3/provider/amazon
    public static async Task<IResult> SetItemWatchProvider()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // PUT app/boards/1/lists/2/items/3/backdrop/4
    public static async Task<IResult> SetItemBackdrop()
    {
        await Task.Yield();
        return Results.Ok();
    }
    
    // POST app/boards/1/lists/2/items?TmDb=123&type=tv|movie
    public static async Task<IResult> AddItem(HttpResponse response, [FromServices] ITmDb tmDb, [FromServices] AppDbContext db, [FromRoute] Guid boardId,
        [FromRoute] Guid listId, [FromQuery] int tmDbId, [FromQuery] string type)
    {
        var tmDbItem = await tmDb.GetDetail(tmDbId, type);
        var images = await tmDb.GetImages(tmDbId, type);
        var dbItem = tmDbItem.MapTo(listId, images);
        db.Items.Add(dbItem);
        await db.SaveChangesAsync();

        response.Headers.Append("HX-Trigger", "newItem");
        return Results.Ok();
    }

    // PUT app/boards/1/lists/2/items/3
    public static async Task<IResult> UpdateItem([FromServices] ITmDb tmDb, [FromServices] AppDbContext db, [FromRoute] Guid boardId,
        [FromRoute] Guid listId, [FromRoute] Guid itemId, [FromQuery] int tmDbId, [FromQuery] string type)
    {
        var tmDbItem = await tmDb.GetDetail(tmDbId, type);
        var images = await tmDb.GetImages(tmDbId, type);
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        dbItem.MapFrom(tmDbItem, images);
        await db.SaveChangesAsync();

        return new RazorComponentResult<Home>(new {Message = "Hello World!"}); // _Item
    }

    // PUT app/boards/1/lists/2/items
    public static async Task<IResult> SortItems(HttpRequest request, [FromServices] AppDbContext db, [FromRoute] Guid boardId,
        [FromRoute] Guid listId)
    {
        var form = await request.ReadFormAsync();
        var itemIdsStr = form["item"];

        var list = await db.Lists
                       .Include(x => x.Items)
                       .FirstOrDefaultAsync(x => x.Id == listId)
                   ?? throw new KeyNotFoundException();

        var itemIds = itemIdsStr
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => Guid.TryParse(x, out var itemIdGuid) ? itemIdGuid : Guid.Empty);

        var dbItems = list.Items;
        dbItems.RemoveAll(x => !itemIds.Contains(x.Id));

        var itemPosition = 0;
        foreach (var itemId in itemIds.Where(x => x != Guid.Empty))
        {
            var dbItem = dbItems.FirstOrDefault(x => x.Id == itemId);
            if (dbItem == null)
            {
                dbItem = db.Items.FirstOrDefault(x => x.Id == itemId);
                if (dbItem != null)
                {
                    dbItem.Order = itemPosition;
                    list.Items.Add(dbItem);
                }
            }
            else
            {
                dbItem.Order = itemPosition;
            }

            itemPosition++;
        }

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}