using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatchBoard.Helpers;
using WatchBoard.Pages;
using WatchBoard.Services.Database;
using Results = Microsoft.AspNetCore.Http.Results;

namespace WatchBoard.Routes;

public static partial class Routes
{
    public static void MapPages(this WebApplication app)
    {
        // MAIN PAGE
        app.MapGet("/", async (
            HttpContext context,
            [FromServices] AppDbContext db,
            [FromQuery] Guid? boardId) =>
        {
            var boards = await db.Boards.AsNoTracking().OrderByDescending(x => x.Name).ToListAsync();
            var selectedBoard = boardId != null
                ? boards.FirstOrDefault(x => x.Id == boardId)
                : context.GetSelectedBoard(boards);

            var lists = selectedBoard == null
                ? []
                : db.Lists.AsNoTracking()
                    .Include(x => x.Items.OrderBy(y => y.Order))
                    .Where(x => x.BoardId == selectedBoard.Id)
                    .OrderBy(x => x.Order)
                    .ToList();

            context.SetBoardId(selectedBoard?.Id);

            if (boardId != null)
                return Results.Redirect("/");

            return new RazorComponentResult<Home>(new
            {
                Lists = lists,
                Boards = boards
            });
        });
    }
}