namespace WatchBoard.Services.Helpers;

public static class ContextHelpers
{
    public static Guid? GetBoardId(this HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("BoardId", out var value))
            return Guid.Parse(value);
        return null;
    }

    public static void SetBoardId(this HttpContext context, Guid? boardId)
    {
        if (boardId == null)
            context.Response.Cookies.Delete("BoardId");
        else
            context.Response.Cookies.Append("BoardId", boardId.ToString()!, new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
    }
}