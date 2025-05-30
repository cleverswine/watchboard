using System.Net.Mime;
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
                var b = context.GetBoardId();

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
                var selectedImage = form["selectedImage"];
                if (int.TryParse(selectedProvider.ToString(), out var selectedProviderId))
                    await repo.SetItemProvider(itemId, selectedProviderId);
                if (Guid.TryParse(selectedImage.ToString(), out var selectedImageId))
                    await repo.SetItemBackdrop(itemId, selectedImageId);

                var v = context.GetViewMode();
                if (v == "posters")
                    return new RazorComponentResult<_ItemPoster>(new
                    {
                        ItemModel = await repo.GetItem(itemId)
                    });
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
            async Task<RazorComponentResult> (HttpContext context, [FromServices] IRepository repo, [FromRoute] Guid itemId) =>
            {
                var v = context.GetViewMode();
                if (v == "posters")
                    return new RazorComponentResult<_ItemPoster>(new
                    {
                        ItemModel = await repo.RefreshItem(itemId)
                    });
                return new RazorComponentResult<_Item>(new
                {
                    ItemModel = await repo.RefreshItem(itemId)
                });
            });

        return app;
    }
}