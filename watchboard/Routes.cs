using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchBoard.Pages;
using WatchBoard.Services;
using WatchBoard.Services.Database;
using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.TmDb;

namespace WatchBoard;

public static class Routes
{
    public static void MapPages(this WebApplication app)
    {
        app.MapGet("/", async ([FromServices] AppDbContext db) =>
        {
            var boards = await db.Boards.AsNoTracking().OrderByDescending(x => x.Name).ToListAsync();
            var selectedBoard = boards.FirstOrDefault();
            var lists = selectedBoard == null
                ? []
                : db.Lists.AsNoTracking()
                    .Include(x => x.Items.OrderBy(y => y.Order))
                    .Where(x => x.BoardId == selectedBoard.Id)
                    .OrderByDescending(x => x.Order)
                    .ToList();

            return new RazorComponentResult<Home>(new {Boards = boards, SelectedBoard = selectedBoard, Lists = lists});
        });
    }

    public static void MapPartials(this RouteGroupBuilder app)
    {
        app.MapGet("/empty", () => Results.Ok());

        // SEARCH
        app.MapPost("/boards/{boardId:guid}/lists/{listId:guid}/search",
            async (HttpRequest request, [FromRoute] Guid boardId, [FromRoute] Guid listId, [FromServices] AppDbContext db, [FromServices] ITmDb tmDb) =>
            {
                var form = await request.ReadFormAsync();
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

                return new RazorComponentResult<_List>(new
                {
                    ListModel = new List {Id = Guid.Empty, Items = items, Name = "Search Results"}, SelectedBoard = boardId,
                    Lists = db.Lists.AsNoTracking().Where(x => x.BoardId == boardId).OrderByDescending(x => x.Order).ToList()
                });
            });

        // GET LIST
        app.MapGet("/boards/{boardId:guid}/lists/{listId:guid}", async ([FromServices] AppDbContext db, [FromRoute] Guid boardId, [FromRoute] Guid listId) =>
        {
            var list = await db.Lists.AsNoTracking()
                .Include(x => x.Items.OrderBy(y => y.Order))
                .FirstOrDefaultAsync(x => x.Id == listId);
            return new RazorComponentResult<_List>(new
            {
                ListModel = list, SelectedBoard = boardId,
                Lists = db.Lists.AsNoTracking().Where(x => x.BoardId == boardId).OrderByDescending(x => x.Order).ToList()
            });
        });

        // SORT LIST
        app.MapPut("/boards/{boardId:guid}/lists/{listId:guid}/items",
            async (HttpRequest request, [FromServices] ITmDb tmDb, [FromServices] AppDbContext db, [FromRoute] Guid listId) =>
            {
                var form = await request.ReadFormAsync();
                var itemIdsStr = form["item"];

                // var dbList = await db.Lists.FirstOrDefaultAsync(x => x.Id == listId) ?? throw new KeyNotFoundException();
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
        app.MapPost("/boards/{boardId:guid}/lists/{listId:guid}/items/{tmDbId:int}", async (HttpResponse response,
            [FromServices] ITmDb tmDb,
            [FromServices] AppDbContext db,
            [FromRoute] Guid boardId,
            [FromRoute] Guid listId,
            [FromRoute] int tmDbId,
            [FromQuery] string type) =>
        {
            var order = db.Items.AsNoTracking().Where(x => x.ListId == listId).Max(x => x.Order) + 1;
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
        app.MapDelete("/items/{id:guid}", async ([FromServices] AppDbContext db, [FromRoute] Guid id) =>
        {
            var item = await db.Items.FindAsync(id);
            if (item == null) return Results.Ok();
            db.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok();
        });

        // UPDATE ITEM
        app.MapPut("/boards/{boardId:guid}/lists/{listId:guid}/items/{itemId:guid}",
            async ([FromServices] ITmDb tmDb, [FromServices] AppDbContext db, [FromRoute] Guid boardId, [FromRoute] Guid listId, [FromRoute] Guid itemId) =>
            {
                var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
                var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                dbItem.MapFrom(tmDbItem, images);
                await db.SaveChangesAsync();

                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = dbItem, SelectedBoard = boardId, SelectedList = listId,
                    Lists = db.Lists.AsNoTracking().Where(x => x.BoardId == boardId).OrderByDescending(x => x.Order).ToList()
                });
            });
    }
}