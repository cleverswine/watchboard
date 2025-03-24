using System.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchBoard.Helpers;
using WatchBoard.Pages;
using WatchBoard.Services;
using WatchBoard.Services.Database;
using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.TmDb;

namespace WatchBoard.Routes;

public static partial class Routes
{
    public static void MapAppPartials(this RouteGroupBuilder app)
    {
        app.MapGet("/empty", () => Results.Ok());

        // SEARCH
        app.MapPost("/search",
            async (HttpContext context,
                [FromServices] AppDbContext db,
                [FromServices] ITmDb tmDb) =>
            {
                var form = await context.Request.ReadFormAsync();
                var s = form["SearchName"];

                var tmDbResults = await tmDb.Search(s!);
                var items = tmDbResults.Select(x => new Item
                {
                    Id = Guid.Empty,
                    Name = x.ItemName ?? "UNKNOWN",
                    Type = x.MediaType == "tv" ? ItemType.Tv : ItemType.Movie,
                    TagLine = x.TagLine,
                    ReleaseDate = x.ItemReleaseDate,
                    EndDate = x.LastAirDate,
                    NumberOfSeasons = x.NumberOfSeasons,
                    TmdbId = x.Id,
                    PosterUrl = x.PosterPath ?? "/img/ph.png",
                    BackdropUrl = x.BackdropPath ?? "/img/ph.png"
                }).ToList();

                return new RazorComponentResult<_SearchResults>(new
                {
                    Items = items,
                    Lists = db.Lists.AsNoTracking()
                        .Where(x => x.BoardId == context.GetBoardId())
                        .OrderByDescending(x => x.Order)
                        .ToList()
                });
            });

        // GET LIST
        app.MapGet("/lists/{listId:guid}", async (
            HttpContext context,
            [FromServices] AppDbContext db,
            [FromRoute] Guid listId) =>
        {
            var list = await db.Lists.AsNoTracking()
                .Where(x => x.BoardId == context.GetBoardId())
                .Include(x => x.Items.OrderBy(y => y.Order))
                .FirstOrDefaultAsync(x => x.Id == listId);

            return new RazorComponentResult<_List>(new
            {
                ListModel = list,
                OtherBoards = await db.Boards.AsNoTracking()
                    .Where(x => x.Id != context.GetBoardId())
                    .OrderByDescending(x => x.Name).ToListAsync()
            });
        });

        // SORT LIST
        app.MapPut("/lists/{listId:guid}/items",
            async (HttpContext context,
                [FromServices] AppDbContext db,
                [FromRoute] Guid listId) =>
            {
                var form = await context.Request.ReadFormAsync();
                var itemIdsStr = form["item"];

                var dbItems = db.Items.Where(x => x.ListId == listId).ToList();

                var newItemIds = itemIdsStr
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => Guid.TryParse(x, out var itemIdGuid) ? itemIdGuid : Guid.Empty);

                var itemPosition = 0;
                foreach (var itemId in newItemIds.Where(x => x != Guid.Empty))
                {
                    var dbItem = dbItems.FirstOrDefault(x => x.Id == itemId);
                    if (dbItem == null)
                    {
                        // new item wasn't in this list
                        dbItem = db.Items.FirstOrDefault(x => x.Id == itemId);
                        if (dbItem != null)
                        {
                            // move it to this list
                            dbItem.Order = itemPosition;
                            dbItem.ListId = listId;
                        }
                    }
                    else
                    {
                        // just set the order
                        dbItem.Order = itemPosition;
                    }

                    itemPosition++;
                }

                await db.SaveChangesAsync();

                return Results.Ok();
            });

        // ADD ITEM
        app.MapPost("/lists/{listId:guid}/items/{tmDbId:int}", async (HttpResponse response,
            [FromServices] ITmDb tmDb,
            [FromServices] AppDbContext db,
            [FromRoute] Guid listId,
            [FromRoute] int tmDbId,
            [FromQuery] string type) =>
        {
            var order = (db.Items.AsNoTracking().Where(x => x.ListId == listId).OrderByDescending(x => x.Order).FirstOrDefault()?.Order ?? -1) + 1;
            var tmDbItem = await tmDb.GetDetail(tmDbId, type);
            var images = await tmDb.GetImages(tmDbId, type);
            var dbItem = tmDbItem.MapTo(listId, images);
            dbItem.BackdropBase64 = await tmDb.GetImageBase64(dbItem.BackdropUrl, "w300");
            dbItem.PosterBase64 = await tmDb.GetImageBase64(dbItem.PosterUrl, "w92");
            dbItem.Order = order;
            db.Items.Add(dbItem);
            await db.SaveChangesAsync();

            response.Headers.Append("HX-Trigger", "newItem");
            return Results.Ok();
        });

        // DELETE ITEM
        app.MapDelete("/items/{itemId:guid}", async (
            [FromServices] AppDbContext db,
            [FromRoute] Guid itemId) =>
        {
            var item = await db.Items.FindAsync(itemId);
            if (item == null) return Results.Ok();
            db.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok();
        });

        // MOVE ITEM TO ANOTHER BOARD
        app.MapPut("/items/{itemId:guid}/move/{boardId:guid}", async (
            [FromServices] AppDbContext db,
            [FromRoute] Guid itemId,
            [FromRoute] Guid boardId) =>
        {
            var item = await db.Items.FindAsync(itemId);
            if (item == null) return Results.Ok();

            var itemList = await db.Lists.FirstOrDefaultAsync(x => x.Id == item.ListId);
            var otherLists = await db.Lists.Where(x => x.BoardId == boardId).ToListAsync();
            var newList = otherLists.FirstOrDefault(x => x.Name.Equals(itemList?.Name, StringComparison.OrdinalIgnoreCase)) ?? otherLists.FirstOrDefault();
            if (newList == null) return Results.Ok();

            item.ListId = newList.Id;
            await db.SaveChangesAsync();

            return Results.Ok();
        });

        // UPDATE SELECTED PROVIDER
        app.MapPut("/items/{itemId:guid}/providers/{providerName}",
            async (
                HttpContext context,
                [FromServices] ITmDb tmDb,
                [FromServices] AppDbContext db,
                [FromRoute] Guid itemId,
                [FromRoute] string providerName) =>
            {
                var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
                dbItem.SelectedProviderName = HttpUtility.UrlDecode(providerName);
                dbItem.Expanded = true;
                await db.SaveChangesAsync();

                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = dbItem,
                    OtherBoards = await db.Boards.AsNoTracking()
                        .Where(x => x.Id != context.GetBoardId())
                        .OrderByDescending(x => x.Name).ToListAsync()
                });
            });

        // UPDATE SELECTED BACKDROP
        app.MapPut("/items/{itemId:guid}/backdrops/{imageId:guid}",
            async (
                HttpContext context,
                [FromServices] ITmDb tmDb,
                [FromServices] AppDbContext db,
                [FromRoute] Guid itemId,
                [FromRoute] Guid imageId) =>
            {
                var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
                var img = dbItem.GetImages().FirstOrDefault(x => x.Id == imageId) ?? throw new KeyNotFoundException();
                dbItem.BackdropBase64 = await tmDb.GetImageBase64(img.UrlPath, "w300");
                dbItem.BackdropUrl = img.UrlPath;
                dbItem.Expanded = true;
                await db.SaveChangesAsync();

                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = dbItem,
                    OtherBoards = await db.Boards.AsNoTracking()
                        .Where(x => x.Id != context.GetBoardId())
                        .OrderByDescending(x => x.Name).ToListAsync()
                });
            });

        // UPDATE ITEM FROM TMDB
        app.MapPut("/items/{itemId:guid}",
            async (
                HttpContext context,
                [FromServices] ITmDb tmDb,
                [FromServices] AppDbContext db,
                [FromRoute] Guid itemId) =>
            {
                var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
                var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                dbItem.MapFrom(tmDbItem, images);
                await db.SaveChangesAsync();

                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = dbItem,
                    OtherBoards = await db.Boards.AsNoTracking()
                        .Where(x => x.Id != context.GetBoardId())
                        .OrderByDescending(x => x.Name).ToListAsync()
                });
            });
    }
}