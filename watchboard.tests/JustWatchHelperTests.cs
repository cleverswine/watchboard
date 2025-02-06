using WatchBoard.Helpers;

namespace WatchBoard.Tests;

public class JustWatchHelperTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task ExtractStuff()
    {
        var result = await JustWatchHelper.ExtractStuff("https://www.justwatch.com/us/tv-show/severance");
    }
}