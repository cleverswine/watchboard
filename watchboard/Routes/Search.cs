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
            var s = form["SearchName"].ToString();
            if (string.IsNullOrWhiteSpace(s)) throw new Exception("Search Name is required.");
            var t = form["SearchType"].ToString();
            if (string.IsNullOrWhiteSpace(t)) t = "tv";

            var type = Enum.Parse<ItemType>(t, true);

            List<Item> items;
            string? errorMessage = null;
            try
            {
                items = await repo.SearchForItems(s, type);
            }
            catch (Exception ex)
            {
                items = new List<Item>();
                errorMessage = ex.ToString();
            }

            return new RazorComponentResult<_SearchResults>(new
            {
                Items = items,
                Lists = new List<List>(),
                ErrorMessage = errorMessage
            });
        });

        return app;
    }
}