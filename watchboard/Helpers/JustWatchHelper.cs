using AngleSharp;
using AngleSharp.Dom;
using WatchBoard.Services.Database.Entities;

namespace WatchBoard.Helpers;

/*
 * https://www.justwatch.com/us/movie/fight-club
 * {"imdbId":"tt0137523","__typename":"ExternalIds"}
 * <title>Fight Club streaming: where to watch movie online?</title>
 * <h1 data-v-7f57001d="" class="title-detail-hero__details__title"> The Substance <span data-v-7f57001d="" class="release-year">(2024)</span></h1>
 */

public static class JustWatchHelper
{
    public static async Task<(ItemType itemType, string? imDbId, string? Name)> ExtractStuff(string url)
    {
        var document = await BrowsingContext
            .New(Configuration.Default.WithDefaultLoader())
            .OpenAsync(url);

        var imDbId = document.Source.Text.Split("{\"imdbId\":\"")[1].Split("\",\"")[0];
        
        var title = document
            .QuerySelectorAll("h1.title-detail-hero__details__title")
            .Select(m => m.InnerHtml)
            .FirstOrDefault()?.Split('<')[0].Trim();

        return (url.Contains("tv-show") ? ItemType.Tv : ItemType.Movie, imDbId, title);
    }
}