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

        return app;
    }
}