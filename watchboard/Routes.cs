using System.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Pages;
using WatchBoard.Pages.Partials;
using WatchBoard.PagesAdmin;
using WatchBoard.Services;
using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.Helpers;
using WatchBoard.Services.TmDb;
using Results = Microsoft.AspNetCore.Http.Results;

namespace WatchBoard;

public static class Routes
{
    public static void MapPageRoutes(this WebApplication app)
    {
        // START FRESH PAGE
        app.MapGet("/start", () => { return new RazorComponentResult<Start>(); });
        
        // ADMIN HOME PAGE
        app.MapGet("/admin", () => { return new RazorComponentResult<Admin>(); });
        
        app.MapGet("/admin/providers", async ([FromServices] IRepository repo) =>
        {
            return new RazorComponentResult<Providers>(new
            {
                ProvidersList = await repo.GetTmDbProviders()
            });
        });
        
        app.MapGet("/admin/boards", async ([FromServices] IRepository repo) =>
        {
            return new RazorComponentResult<Boards>(new
            {
                BoardModels = await repo.GetBoards()
            });
        });
        
        // HOME PAGE
        app.MapGet("/", async (HttpContext context, [FromServices] IRepository repo, [FromQuery] Guid? boardId) =>
        {
            var bid = boardId ?? context.GetBoardId();
            var selectedBoard = await repo.GetBoardById(bid);

            context.SetBoardId(selectedBoard?.Id);

            if (boardId != null)
                return Results.Redirect("/");

            return new RazorComponentResult<Home>(new
            {
                selectedBoard?.Lists,
                Boards = await repo.GetBoards()
            });
        });

        // EMPTY PAGE
        app.MapGet("/empty", () => Results.Ok());
    }

    public static RouteGroupBuilder MapAppPartials(this RouteGroupBuilder app)
    {
        // SEARCH
        app.MapPost("/search", async (HttpContext context, [FromServices] IRepository repo, [FromServices] ITmDb tmDb) =>
        {
            var form = await context.Request.ReadFormAsync();
            var s = form["SearchName"];

            return new RazorComponentResult<_SearchResults>(new
            {
                Items = await repo.Search(s!),
                Lists = new List<List>()
            });
        });

        // GET LIST
        app.MapGet("/lists/{listId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid listId) => new RazorComponentResult<_List>(new
        {
            ListModel = await repo.GetListById(listId)
        }));

        // SORT LIST
        app.MapPut("/lists/{listId:guid}/items", async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid listId) =>
        {
            var form = await context.Request.ReadFormAsync();
            await repo.SortList(listId, form["item"].ToArray());
            return Results.Ok();
        });

        // ADD ITEM
        app.MapPost("/lists/{listId:guid}/items/{tmDbId:int}", async (HttpResponse response, [FromServices] IRepository repo, [FromRoute] Guid listId,
            [FromRoute] int tmDbId, [FromQuery] string type) =>
        {
            await repo.AddItem(listId, tmDbId, type);
            response.Headers.Append("HX-Trigger", "newItem");
            return Results.Ok();
        });

        // ADD ITEM
        app.MapPost("/items/{tmDbId:int}",
            async (HttpContext context, HttpResponse response, [FromServices] IRepository repo, [FromRoute] int tmDbId, [FromQuery] string type) =>
            {
                await repo.AddItemToBoard(context.GetBoardId(), tmDbId, type);
                response.Headers.Append("HX-Trigger", "newItem");
                return Results.Ok();
            });

        // DELETE ITEM
        app.MapDelete("/items/{itemId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid itemId) =>
        {
            await repo.DeleteItem(itemId);
            return Results.Ok();
        });

        // EDIT ITEM
        app.MapGet("/items/{itemId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid itemId) =>
        {
            var item = await repo.GetItemById(itemId);
            if (item == null) throw new KeyNotFoundException();
            return new RazorComponentResult<_ItemDetail>(new
            {
                ItemModel = item,
                Boards = (await repo.GetBoards()).Where(x => x.Lists.All(l => l.Id != item.ListId)).ToList()
            });
        });

        // MOVE ITEM TO ANOTHER BOARD
        app.MapPut("/items/{itemId:guid}/move/{boardId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid itemId, [FromRoute] Guid boardId) =>
        {
            await repo.MoveItem(itemId, boardId);
            return Results.Ok();
        });

        // UPDATE ITEM
        app.MapPut("/items/{itemId:guid}",
            async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid itemId) =>
            {
                var form = await context.Request.ReadFormAsync();
                var s = form["selectedProvider"];
                var i = form["selectedImage"];
                await repo.SetProvider(itemId, s!);
                var item = await repo.SetBackdrop(itemId, Guid.Parse(i!));
                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = item
                });
            });

        // UPDATE SELECTED PROVIDER
        app.MapPut("/items/{itemId:guid}/providers/{providerName}",
            async ([FromServices] IRepository repo, [FromRoute] Guid itemId, [FromRoute] string providerName) => new RazorComponentResult<_Item>(new
            {
                ItemModel = await repo.SetProvider(itemId, HttpUtility.HtmlDecode(providerName))
            }));

        // UPDATE SELECTED BACKDROP
        app.MapPut("/items/{itemId:guid}/backdrops/{imageId:guid}",
            async ([FromServices] IRepository repo, [FromRoute] Guid itemId, [FromRoute] Guid imageId) => new RazorComponentResult<_Item>(new
            {
                ItemModel = await repo.SetBackdrop(itemId, imageId)
            }));

        // GET TMDB IMG BYTES
        app.MapGet("/tmdb/images/{imgUrl}", async ([FromRoute] string imgUrl, [FromServices] ITmDb tmDb) =>
        {
            var b = await tmDb.GetImageBytes("/" + imgUrl);
            return Results.File(b, "image/" + imgUrl.Split(".")[1]);
        });

        // UPDATE ITEM FROM TMDB
        app.MapPut("/items/{itemId:guid}/refresh",
            async ([FromServices] IRepository repo, [FromRoute] Guid itemId) => new RazorComponentResult<_Item>(new
            {
                ItemModel = await repo.RefreshItem(itemId)
            }));

        return app;
    }
    
    public static void MapAdminPartials(this RouteGroupBuilder app)
    {

    }
}