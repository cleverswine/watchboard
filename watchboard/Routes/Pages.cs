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

        // EMPTY PAGE
        app.MapGet("/empty", () => Results.Ok());
        
        return app;
    }
}