using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services;

public static class Mapping
{
    public static void MapFrom(this Item item, TmdbItem tmDbItem, ImageList imageList)
    {
        var providerNames = (tmDbItem.Providers?.Results.Us.FlatRate ?? []).Select(x => x.ProviderName).ToList();
        providerNames.Insert(0, "Servarr");

        item.Name = tmDbItem.Name ?? item.Name;
        item.Overview = tmDbItem.Overview;
        item.TagLine = tmDbItem.TagLine;
        item.ReleaseDate = tmDbItem.ItemReleaseDate;
        item.EndDate = tmDbItem.LastAirDate;
        item.NumberOfSeasons = tmDbItem.NumberOfSeasons;
        item.ImdbId = tmDbItem.ExternalIds.ImdbId;
        item.TmdbId = tmDbItem.Id;
        item.ProviderNamesCsv = string.Join(',', providerNames);
        item.SeriesStatus = tmDbItem.MapToSeriesStatus();
        item.SeriesNextEpisodeDate = tmDbItem.NextEpisodeToAir?.AirDate;
        item.SeriesNextEpisodeNumber = tmDbItem.NextEpisodeToAir?.EpisodeNumber;
        item.SeriesNextEpisodeSeason = tmDbItem.NextEpisodeToAir?.SeasonNumber;
        item.SetImages(imageList.MapTo());
    }

    private static SeriesStatus? MapToSeriesStatus(this TmdbItem tmDbItem)
    {
        if (tmDbItem.MediaType != "tv") return null;
        if (tmDbItem.Status == null) return null;
        if (tmDbItem.Status.Equals("Ended", StringComparison.OrdinalIgnoreCase)) return SeriesStatus.Ended;
        if (tmDbItem.Status.Equals("Returning Series", StringComparison.OrdinalIgnoreCase))
        {
            return tmDbItem.NextEpisodeToAir == null ? SeriesStatus.Returning : SeriesStatus.InProgress;
        }

        return null;
    }

    public static Item MapTo(this TmdbItem tmDbItem, Guid listId, ImageList imageList)
    {
        var providerNames = (tmDbItem.Providers?.Results.Us.FlatRate ?? []).Select(x => x.ProviderName).ToList();
        providerNames.Insert(0, "Servarr");

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
            Expanded = false,
            ProviderNamesCsv = string.Join(',', providerNames),
            SelectedProviderName = null,
            ListId = listId,
            SeriesStatus = tmDbItem.MapToSeriesStatus(),
            SeriesNextEpisodeDate = tmDbItem.NextEpisodeToAir?.AirDate,
            SeriesNextEpisodeNumber = tmDbItem.NextEpisodeToAir?.EpisodeNumber,
            SeriesNextEpisodeSeason = tmDbItem.NextEpisodeToAir?.SeasonNumber
        };
        item.SetImages(imageList.MapTo());

        return item;
    }

    private static List<ItemImage> MapTo(this ImageList imageList)
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
}