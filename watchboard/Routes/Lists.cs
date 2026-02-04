using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Pages.Partials;
using WatchBoard.Services;

namespace WatchBoard.Routes;

public static class Lists
{
    public static RouteGroupBuilder MapLists(this RouteGroupBuilder app)
    {
        // GET LIST
        app.MapGet("/lists/{listId:guid}", async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid listId) =>
        new RazorComponentResult<_List>(new
        {
            ListModel = await repo.GetList(listId)
        }));

        // GET LIST ITEM COUNT
        app.MapGet("/lists/{listId:guid}/items/count", async (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid listId) =>
        (await repo.GetListItemCount(listId)).ToString());

        // SORT LIST
        app.MapPut("/lists/{listId:guid}/items", async (HttpResponse response, HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid listId) =>
        {
            var form = await context.Request.ReadFormAsync();
            await repo.SortList(listId, form["item"].ToArray());
            response.Headers.Append("HX-Trigger", "moveItem");
            return Results.Ok();
        });

        // MOVE ITEM TO ANOTHER LIST
        app.MapPut("/items/{itemId:guid}/move/lists/{listId:guid}",
            async ([FromServices] IRepository repo, [FromRoute] Guid itemId, [FromRoute] Guid listId) =>
            {
                await repo.MoveItemToOtherList(itemId, listId);
                var details = await repo.GetItemWithDetails(itemId);
                return new RazorComponentResult<_SettingsItemRow>(new
                {
                    ItemModel = details?.Item,
                    BoardName = details?.BoardName ?? "Unknown",
                    ListName = details?.ListName ?? "Unknown"
                });
            });

        return app;
    }
}