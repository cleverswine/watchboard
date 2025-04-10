using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Pages;
using WatchBoard.Pages.Partials;
using WatchBoard.PagesAdmin;
using WatchBoard.PagesAdmin.Partials;
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
            var selectedBoard = await repo.GetBoard(bid);

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

    public static void MapAppPartials(this RouteGroupBuilder app)
    {
        // SEARCH
        app.MapPost("/search", async (HttpContext context, [FromServices] IRepository repo, [FromServices] ITmDb tmDb) =>
        {
            var form = await context.Request.ReadFormAsync();
            var s = form["SearchName"];

            return new RazorComponentResult<_SearchResults>(new
            {
                Items = await repo.SearchForItems(s!),
                Lists = new List<List>()
            });
        });

        // GET LIST
        app.MapGet("/lists/{listId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid listId) => new RazorComponentResult<_List>(new
        {
            ListModel = await repo.GetList(listId)
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
            var item = await repo.GetItem(itemId);
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
            await repo.MoveItemToOtherBoard(itemId, boardId);
            return Results.Ok();
        });

        // UPDATE ITEM
        app.MapPut("/items/{itemId:guid}",
            async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid itemId) =>
            {
                var form = await context.Request.ReadFormAsync();
                var selectedProvider = form["selectedProvider"];
                var selectedImage = form["selectedImage"];
                if (int.TryParse(selectedProvider.ToString(), out var selectedProviderId))
                    await repo.SetItemProvider(itemId, selectedProviderId);
                if (Guid.TryParse(selectedImage.ToString(), out var selectedImageId))
                    await repo.SetItemBackdrop(itemId, selectedImageId);

                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = await repo.GetItem(itemId)
                });
            });

        // GET TMDB BACKDROP IMAGE TAG
        app.MapGet("/items/{itemId:guid}/backdrops/{imageId:guid}",
            async ([FromServices] IRepository repo, [FromRoute] Guid itemId, [FromRoute] Guid imageId) =>
            {
                var url = await repo.GetItemBackdropUrl(itemId, imageId);
                var s =
                    $"<img class=\"img-thumbnail\" src=\"{url}\" width=\"200\" height=\"112\" alt=\"{imageId}\"/>";
                return Results.Content(s, MediaTypeNames.Text.Html);
            });

        // UPDATE ITEM FROM TMDB
        app.MapPut("/items/{itemId:guid}/refresh",
            async ([FromServices] IRepository repo, [FromRoute] Guid itemId) => new RazorComponentResult<_Item>(new
            {
                ItemModel = await repo.RefreshItem(itemId)
            }));
    }

    public static void MapAdminPages(this RouteGroupBuilder app)
    {
        // ADMIN PAGES
        app.MapGet("/", () => new RazorComponentResult<Admin>());

        app.MapGet("/manageProviders", async ([FromServices] IRepository repo) => new RazorComponentResult<Providers>(new
        {
            ProvidersList = await repo.GetTmDbProviders()
        }));

        app.MapGet("/manageBoards", async ([FromServices] IRepository repo) => new RazorComponentResult<Boards>(new
        {
            BoardModels = await repo.GetBoards()
        }));

        // GET BOARD ROW
        app.MapGet("/boards/{boardId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid boardId) =>
        new RazorComponentResult<_BoardRow>(new
        {
            BoardModel = await repo.GetBoard(boardId)
        }));

        // GET BOARD ROW EDIT
        app.MapGet("/boards/{boardId:guid}/edit", async ([FromServices] IRepository repo, [FromRoute] Guid boardId) =>
        new RazorComponentResult<_BoardRowEdit>(new
        {
            BoardModel = await repo.GetBoard(boardId)
        }));

        // SAVE BOARD ROW
        app.MapPut("/boards/{boardId:guid}", async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid boardId) =>
        {
            var form = await context.Request.ReadFormAsync();
            var s = form["BoardName"];
            return new RazorComponentResult<_BoardRow>(new
            {
                BoardModel = await repo.UpdateBoard(boardId, s.ToString())
            });
        });

        // DELETE BOARD ROW
        app.MapDelete("/boards/{boardId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid boardId) =>
        {
            await repo.DeleteBoard(boardId);
            return Results.Ok();
        });

        // ADD BOARD ROW
        app.MapPost("/boards", async (HttpContext context, [FromServices] IRepository repo) =>
        {
            var form = await context.Request.ReadFormAsync();
            var s = form["BoardName"];
            return new RazorComponentResult<_BoardRow>(new
            {
                BoardModel = await repo.AddBoard(s.ToString())
            });
        });

        // GET LIST ROW
        app.MapGet("/boards/{boardId:guid}/lists/{listId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid boardId, [FromRoute] Guid listId) =>
        new RazorComponentResult<_ListRow>(new
        {
            ListModel = await repo.GetList(listId)
        }));

        // GET LIST ROW EDIT
        app.MapGet("/boards/{boardId:guid}/lists/{listId:guid}/edit",
            async ([FromServices] IRepository repo, [FromRoute] Guid boardId, [FromRoute] Guid listId) =>
            new RazorComponentResult<_ListRowEdit>(new
            {
                ListModel = await repo.GetList(listId)
            }));

        // SAVE LIST ROW
        app.MapPut("/boards/{boardId:guid}/lists/{listId:guid}",
            async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid boardId, [FromRoute] Guid listId) =>
            {
                var form = await context.Request.ReadFormAsync();
                var s = form["ListName"];
                return new RazorComponentResult<_ListRow>(new
                {
                    ListModel = await repo.UpdateList(listId, s.ToString())
                });
            });

        // DELETE LIST ROW
        app.MapDelete("/boards/{boardId:guid}/lists/{listId:guid}",
            async ([FromServices] IRepository repo, [FromRoute] Guid boardId, [FromRoute] Guid listId) =>
            {
                await repo.DeleteList(listId);
                return Results.Ok();
            });

        // ADD LIST ROW
        app.MapPost("/boards/{boardId:guid}/lists", async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid boardId) =>
        {
            var form = await context.Request.ReadFormAsync();
            var s = form["ListName"];
            await repo.AddList(s.ToString());
            return new RazorComponentResult<_Board>(new
            {
                BoardModel = await repo.GetBoard(boardId)
            });
        });

        // MOVE LIST ROW UP
        app.MapPut("/boards/{boardId:guid}/lists/{listId:guid}/up",
            async ([FromServices] IRepository repo, [FromRoute] Guid boardId, [FromRoute] Guid listId) =>
            {
                await repo.MoveListUp(listId);
                return new RazorComponentResult<_Board>(new
                {
                    BoardModel = await repo.GetBoard(boardId)
                });
            });

        // MOVE LIST ROW DOWN
        app.MapPut("/boards/{boardId:guid}/lists/{listId:guid}/down",
            async ([FromServices] IRepository repo, [FromRoute] Guid boardId, [FromRoute] Guid listId) =>
            {
                await repo.MoveListDown(listId);
                return new RazorComponentResult<_Board>(new
                {
                    BoardModel = await repo.GetBoard(boardId)
                });
            });
    }
}