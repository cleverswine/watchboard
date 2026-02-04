using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Pages;
using WatchBoard.Services;
using WatchBoard.Services.Helpers;
using Results = Microsoft.AspNetCore.Http.Results;

namespace WatchBoard.Routes;

public static class Pages
{
    public static WebApplication MapPages(this WebApplication app)
    {
        // HOME PAGE
        app.MapGet("/", async (HttpContext context, [FromServices] IRepository repo, [FromQuery] string? v, [FromQuery] Guid? boardId) =>
        {
            var bid = boardId ?? context.GetBoardId();
            var selectedBoard = await repo.GetBoard(bid);
            context.SetBoardId(selectedBoard?.Id);

            return new RazorComponentResult<Home>(new
            {
                selectedBoard?.Lists,
                Boards = await repo.GetBoards()
            });
        });

        // SETTINGS PAGE
        app.MapGet("/settings", async ([FromServices] IRepository repo) =>
        {
            var items = await repo.GetAllItemsWithDetails();
            var boards = await repo.GetBoards();
            return new RazorComponentResult<Settings>(new
            {
                Items = items,
                Boards = boards
            });
        });


        // EMPTY PAGE
        app.MapGet("/empty", () => Results.Ok());
        
        // UPDATE ALL ITEMS FROM TMDB
        app.MapPut("/refresh", async (HttpContext context, [FromServices] IRepository repo, CancellationToken cancellationToken) =>
        {
            await repo.RefreshAllItems(15, cancellationToken);
            
            var selectedBoard = await repo.GetBoard(context.GetBoardId());
            return new RazorComponentResult<Home>(new
            {
                selectedBoard?.Lists,
                Boards = await repo.GetBoards()
            });
        });
        
        return app;
    }
}