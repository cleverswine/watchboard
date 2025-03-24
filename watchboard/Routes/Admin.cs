using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WatchBoard.Pages;
using WatchBoard.Services.TmDb;

namespace WatchBoard.Routes;

public static partial class Routes
{
    public static void MapAdmin(this RouteGroupBuilder app)
    {
        // MAIN ADMIN PAGE
        app.MapGet("/admin", async ([FromServices] ITmDb tnDb) =>
        {
            return new RazorComponentResult<Admin>(new
            {
                ProvidersList = await tnDb.GetProviders("tv")
            });
        });

        // GET ALL BOARDS
        app.MapGet("/boards", () => Results.Ok());
        // REORDER ALL BOARDS
        app.MapPut("/boards", () => Results.Ok());

        // CREATE A NEW BOARD
        app.MapPost("/boards", () => Results.Ok());
        // GET A BOARD
        app.MapGet("/boards/{boardId:guid}", () => Results.Ok());
        // UPDATE A BOARD
        app.MapPut("/boards/{boardId:guid}", () => Results.Ok());
        // DELETE A BOARD
        app.MapDelete("/boards/{boardId:guid}", () => Results.Ok());

        // REORDER BOARD LISTS
        app.MapPut("/boards/{boardId:guid}/lists", () => Results.Ok());

        // CREATE A BOARD LIST
        app.MapPost("/boards/{boardId:guid}/lists", () => Results.Ok());
        // UPDATE A BOARD LIST
        app.MapPut("/boards/{boardId:guid}/lists/{listId:guid}", () => Results.Ok());
        // DELETE A BOARD LIST
        app.MapDelete("/boards/{boardId:guid}/lists/{listId:guid}", () => Results.Ok());
    }
}