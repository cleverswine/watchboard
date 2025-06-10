using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Pages.Partials;
using WatchBoard.Services;
using WatchBoard.Services.Helpers;

namespace WatchBoard.Routes;

public static class Items
{
    public static RouteGroupBuilder MapItems(this RouteGroupBuilder app)
    {
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

        // VIEW ITEM
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
            async Task<RazorComponentResult> (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid itemId) =>
            {
                var form = await context.Request.ReadFormAsync();
                var selectedProvider = form["selectedProvider"];
                if (int.TryParse(selectedProvider.ToString(), out var selectedProviderId))
                    await repo.SetItemProvider(itemId, selectedProviderId);
                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = await repo.GetItem(itemId)
                });
            });

        // UPDATE ITEM FROM TMDB
        app.MapPut("/items/{itemId:guid}/refresh",
            async Task<RazorComponentResult> ([FromServices] IRepository repo, [FromRoute] Guid itemId) => new RazorComponentResult<_Item>(new
            {
                ItemModel = await repo.RefreshItem(itemId)
            }));

        return app;
    }
}