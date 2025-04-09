using Humanizer;
using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services;

public static class Mapping
{
    public static string HumanizeDateString(this string dateTime)
    {
        var now = DateTime.UtcNow;
        if (!DateTime.TryParse(dateTime, out var dateTimeResult)) return dateTime;
        return Math.Abs(dateTimeResult.ToUniversalTime().Subtract(now).TotalHours) < 24
            ? "today!"
            : dateTimeResult.Humanize(true, now.AddDays(-1));
    }

    public static void UpdateFromTmDb(this Item item, TmDbItem tmDbItem, TmDbItemImageList imageList)
    {
        item.Name = tmDbItem.Name ?? item.Name;
        item.Overview = tmDbItem.Overview;
        item.TagLine = tmDbItem.TagLine;
        item.ReleaseDate = tmDbItem.ItemReleaseDate;
        item.EndDate = tmDbItem.LastAirDate;
        item.NumberOfSeasons = tmDbItem.NumberOfSeasons;
        item.ImdbId = tmDbItem.ExternalIds.ImdbId;
        item.TmdbId = tmDbItem.Id;
        item.SeriesStatus = tmDbItem.MapTmDbToSeriesStatus();
        item.SeriesNextEpisodeDate = tmDbItem.NextEpisodeToAir?.AirDate;
        item.SeriesNextEpisodeNumber = tmDbItem.NextEpisodeToAir?.EpisodeNumber;
        item.SeriesNextEpisodeSeason = tmDbItem.NextEpisodeToAir?.SeasonNumber;
        item.SetImages(imageList.MapTmDbToImageList());
        item.SetProviders(tmDbItem.Providers?.MapTmDbToItemProviders() ?? []);
    }

    public static Item MapTmDbToItem(this TmDbItem tmDbItem, Guid listId, TmDbItemImageList imageList)
    {
        var item = new Item
        {
            Name = tmDbItem.ItemName ?? "UNKNOWN",
            Type = tmDbItem.MediaType == "tv"
                ? ItemType.Tv
                : ItemType.Movie,
            Overview = tmDbItem.Overview,
            TagLine = tmDbItem.TagLine,
            ReleaseDate = tmDbItem.ItemReleaseDate,
            EndDate = tmDbItem.LastAirDate,
            NumberOfSeasons = tmDbItem.NumberOfSeasons,
            Order = 0,
            BackdropUrl = imageList.Backdrops.FirstOrDefault()?.FilePath ?? string.Empty,
            BackdropBase64 = null,
            PosterUrl = imageList.Posters.FirstOrDefault()?.FilePath ?? string.Empty,
            PosterBase64 = null,
            ImdbId = tmDbItem.ExternalIds.ImdbId,
            TmdbId = tmDbItem.Id,
            ListId = listId,
            SeriesStatus = tmDbItem.MapTmDbToSeriesStatus(),
            SeriesNextEpisodeDate = tmDbItem.NextEpisodeToAir?.AirDate,
            SeriesNextEpisodeNumber = tmDbItem.NextEpisodeToAir?.EpisodeNumber,
            SeriesNextEpisodeSeason = tmDbItem.NextEpisodeToAir?.SeasonNumber
        };
        item.SetImages(imageList.MapTmDbToImageList());
        item.SetProviders(tmDbItem.Providers?.MapTmDbToItemProviders() ?? []);
        
        return item;
    }

    private static List<ItemProvider> MapTmDbToItemProviders(this TmDbWatchProviders watchProviders)
    {
        var itemProviders = watchProviders.Results.Us.FlatRate.Select(x => new ItemProvider
        {
            Id = x.ProviderId, Name = x.ProviderName ?? x.ProviderId.ToString(), DisplayPriority = x.DisplayPriority
        }).ToList();

        itemProviders.Insert(0, new ItemProvider
        {
            Id = -1, Name = "Home", DisplayPriority = -1
        });

        return itemProviders;
    }

    private static List<ItemImage> MapTmDbToImageList(this TmDbItemImageList imageList)
    {
        var images = imageList.Backdrops.Select(x => new ItemImage
        {
            Id = Guid.NewGuid(),
            Type = ItemImageType.Backdrop,
            Name = x.FilePath ?? "",
            UrlPath = x.FilePath ?? "",
            VoteAverage = x.VoteAverage,
            VoteCount = x.VoteCount
        }).ToList();

        images.AddRange(imageList.Posters.Select(x => new ItemImage
        {
            Id = Guid.NewGuid(),
            Type = ItemImageType.Poster,
            Name = x.FilePath ?? "",
            UrlPath = x.FilePath ?? "",
            VoteAverage = x.VoteAverage,
            VoteCount = x.VoteCount
        }));

        images.AddRange(imageList.Logos.Select(x => new ItemImage
        {
            Id = Guid.NewGuid(),
            Type = ItemImageType.Logo,
            Name = x.FilePath ?? "",
            UrlPath = x.FilePath ?? "",
            VoteAverage = x.VoteAverage,
            VoteCount = x.VoteCount
        }));

        return images;
    }

    private static SeriesStatus? MapTmDbToSeriesStatus(this TmDbItem tmDbItem)
    {
        if (tmDbItem.MediaType != "tv") return null;
        if (tmDbItem.Status == null) return null;
        if (tmDbItem.Status.Equals("Ended", StringComparison.OrdinalIgnoreCase)) return SeriesStatus.Ended;
        if (tmDbItem.Status.Equals("Returning Series", StringComparison.OrdinalIgnoreCase))
            return tmDbItem.NextEpisodeToAir == null ? SeriesStatus.Returning : SeriesStatus.InProgress;

        return null;
    }
}