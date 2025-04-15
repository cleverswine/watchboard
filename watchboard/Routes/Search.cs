using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Database.Entities;
using WatchBoard.Pages.Partials;
using WatchBoard.Services;
using WatchBoard.Services.TmDb;

namespace WatchBoard.Routes;

public static class Search
{
    public static RouteGroupBuilder MapSearch(this RouteGroupBuilder app)
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
        
        return app;
    }
}