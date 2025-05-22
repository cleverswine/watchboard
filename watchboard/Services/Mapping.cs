using Humanizer;
using WatchBoard.Database.Entities;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services;

public static class Mapping
{
    public static string HumanizeDateString(this string dateTime)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var then = DateOnly.Parse(dateTime);
        return now.CompareTo(then) == 0 ? "today!" : then.Humanize();
    }

    public static void UpdateFromTmDb(this Item item, TmDbItem tmDbItem, TmDbImages imageList)
    {
        var selectedProvider = item.GetProviders().FirstOrDefault(x => x.Selected);
        var updatedProviders = tmDbItem.Providers?.MapTmDbToItemProviders() ?? [];
        if (selectedProvider != null)
        {
            var updatedSelectedProvider = updatedProviders.FirstOrDefault(x => x.Id == selectedProvider.Id);
            if (updatedSelectedProvider != null) updatedSelectedProvider.Selected = true;
        }

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
        item.OriginalLanguage = tmDbItem.OriginalLanguage?.ToUpper() ?? "";
        item.OriginCountry = string.Join(", ", tmDbItem.OriginCountry);
        item.Notes = tmDbItem.GetNotes();
        item.SetImages(imageList.MapTmDbToImageList());
        item.SetProviders(updatedProviders);
    }

    public static Item MapTmDbToItem(this TmDbItem tmDbItem, Guid listId, TmDbImages imageList)
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
            SeriesNextEpisodeSeason = tmDbItem.NextEpisodeToAir?.SeasonNumber,
            OriginalLanguage = tmDbItem.OriginalLanguage?.ToUpper() ?? "",
            OriginCountry = string.Join(", ", tmDbItem.OriginCountry),
            Notes = tmDbItem.GetNotes()
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
            Id = -2, Name = "Television", DisplayPriority = -2
        });
        
        itemProviders.Insert(0, new ItemProvider
        {
            Id = -1, Name = "Home Server", DisplayPriority = -1
        });

        return itemProviders;
    }

    private static List<ItemImage> MapTmDbToImageList(this TmDbImages imageList)
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