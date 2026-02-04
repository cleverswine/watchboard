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

        return app;
    }
}
