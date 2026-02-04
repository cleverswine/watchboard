using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Pages.Partials;
using WatchBoard.Services;

namespace WatchBoard.Routes;

public static class SettingsRoutes
{
    public static RouteGroupBuilder MapSettings(this RouteGroupBuilder app)
    {
        var settings = app.MapGroup("/settings");

        // GET DATA REFRESH (items table)
        settings.MapGet("/data-refresh", async ([FromServices] IRepository repo) =>
        {
            var items = await repo.GetAllItemsWithDetails();
            var boards = await repo.GetBoards();
            return new RazorComponentResult<_SettingsItemTable>(new
            {
                Items = items,
                Boards = boards
            });
        });

        // GET SYSTEM LOGS (paged)
        settings.MapGet("/system-logs", async ([FromServices] IRepository repo, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        {
            var (logs, totalCount) = await repo.GetSystemLogs(page, pageSize);
            return new RazorComponentResult<_SettingsSystemLogs>(new
            {
                Logs = logs,
                CurrentPage = page,
                TotalCount = totalCount,
                PageSize = pageSize
            });
        });

        // GET MANAGE BOARDS
        settings.MapGet("/manage-boards", async ([FromServices] IRepository repo) =>
        {
            var boards = await repo.GetBoards();
            return new RazorComponentResult<_SettingsManageBoards>(new
            {
                Boards = boards
            });
        });

        // CREATE BOARD
        settings.MapPost("/boards", async ([FromServices] IRepository repo, [FromBody] CreateBoardRequest request) =>
        {
            var board = await repo.CreateBoard(request.Name);
            return new RazorComponentResult<_SettingsBoardRow>(new
            {
                BoardModel = board
            });
        });

        // REORDER BOARDS
        settings.MapPut("/boards/reorder", async ([FromServices] IRepository repo, [FromBody] Guid[] boardIds) =>
        {
            await repo.ReorderBoards(boardIds);
            return Results.Ok();
        });

        // RENAME BOARD
        settings.MapPut("/boards/{boardId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid boardId, [FromBody] RenameBoardRequest request) =>
        {
            await repo.RenameBoard(boardId, request.Name);
            return Results.Ok();
        });

        // DELETE BOARD
        settings.MapDelete("/boards/{boardId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid boardId) =>
        {
            await repo.DeleteBoard(boardId);
            return Results.Ok();
        });

        // GET MANAGE LISTS
        settings.MapGet("/manage-lists", async ([FromServices] IRepository repo) =>
        {
            var boards = await repo.GetBoards();
            return new RazorComponentResult<_SettingsManageLists>(new
            {
                Boards = boards
            });
        });

        // CREATE LIST
        settings.MapPost("/lists", async ([FromServices] IRepository repo, [FromQuery] Guid boardId, [FromBody] CreateListRequest request) =>
        {
            var list = await repo.CreateList(boardId, request.Name);
            return new RazorComponentResult<_SettingsListRow>(new
            {
                ListModel = list,
                BoardName = ""
            });
        });

        // REORDER LISTS
        settings.MapPut("/lists/reorder", async ([FromServices] IRepository repo, [FromQuery] Guid boardId, [FromBody] Guid[] listIds) =>
        {
            await repo.ReorderLists(boardId, listIds);
            return Results.Ok();
        });

        // RENAME LIST
        settings.MapPut("/lists/{listId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid listId, [FromBody] RenameListRequest request) =>
        {
            await repo.RenameList(listId, request.Name);
            return Results.Ok();
        });

        // SET DEFAULT LIST
        settings.MapPut("/lists/{listId:guid}/default", async ([FromServices] IRepository repo, [FromRoute] Guid listId) =>
        {
            await repo.SetDefaultList(listId);
            return Results.Ok();
        });

        // DELETE LIST
        settings.MapDelete("/lists/{listId:guid}", async ([FromServices] IRepository repo, [FromRoute] Guid listId) =>
        {
            await repo.DeleteList(listId);
            return Results.Ok();
        });

        return app;
    }

    public record RenameBoardRequest(string Name);
    public record CreateBoardRequest(string Name);
    public record RenameListRequest(string Name);
    public record CreateListRequest(string Name);
}
