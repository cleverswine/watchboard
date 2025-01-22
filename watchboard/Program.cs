using WatchBoard.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "Static"
});
builder.Services.AddRazorComponents();

var app = builder.Build();
app.UseStaticFiles();

app.MapGet("/", Routes.Home);

var appBase = app.MapGroup("/app");
// POST app/search
// GET app/boards
appBase.MapGet("/boards", Routes.Home);
// GET app/boards/1
// GET app/boards/1/lists
// GET app/boards/1/lists/2
// GET app/boards/1/lists/2/items
// PUT app/boards/1/lists/2/items (sort)
// GET app/boards/1/lists/2/items/3
// PUT app/boards/1/lists/2/items/3
// DELETE app/boards/1/lists/2/items/3
// PUT app/boards/1/lists/2/items/3/provider/amazon
// PUT app/boards/1/lists/2/items/3/backdrop/4
// POST app/boards/1/lists/2/items?TmDb=123&type=tv|movie

app.Run();
