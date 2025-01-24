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
                    .Include(x => x.Items)
                    .Where(x => x.BoardId == selectedBoard.Id)
                    .ToList();

            return new RazorComponentResult<Home>(new {Boards = boards, SelectedBoard = selectedBoard, Lists = lists});
        });
    }

    public static void MapPartials(this RouteGroupBuilder app)
    {
        // SEARCH
        app.MapPost("/search", async (HttpRequest request, [FromServices] ITmDb tmDb) =>
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

            return new RazorComponentResult<_SearchResults>(new {Items = items});
        });

        // SORT LIST
        app.MapPut("/lists/{listId:guid}/items",
            async (HttpRequest request, [FromServices] ITmDb tmDb, [FromServices] AppDbContext db, [FromRoute] Guid listId) =>
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
            });

        // ADD ITEM
        app.MapPost("/lists/{listId:guid}/items", async (HttpResponse response, [FromServices] ITmDb tmDb, [FromServices] AppDbContext db,
            [FromRoute] Guid listId, [FromQuery] int tmDbId, [FromQuery] string type) =>
        {
            var tmDbItem = await tmDb.GetDetail(tmDbId, type);
            var images = await tmDb.GetImages(tmDbId, type);
            var dbItem = tmDbItem.MapTo(listId, images);
            db.Items.Add(dbItem);
            await db.SaveChangesAsync();

            response.Headers.Append("HX-Trigger", "newItem");
            return Results.Ok();
        });

        // UPDATE ITEM
        app.MapPut("/lists/{listId:guid}/items/{itemId:guid}",
            async ([FromServices] ITmDb tmDb, [FromServices] AppDbContext db, [FromRoute] Guid listId, [FromRoute] Guid itemId) =>
            {
                var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
                var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
                dbItem.MapFrom(tmDbItem, images);
                await db.SaveChangesAsync();

                return new RazorComponentResult<Home>(new {Message = "Hello World!"}); // _Item
            });
    }
}