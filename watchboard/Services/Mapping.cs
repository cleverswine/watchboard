using Humanizer;
using WatchBoard.Database.Entities;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services;

public static class Mapping
{
    public static void UpdateFromTmDb(this Item item, TmDbItem tmDbItem, TmDbImages imageList, List<TmDbSeason> seasons)
    {
        var selectedProvider = item.GetProviders().FirstOrDefault(x => x.Selected);
        var updatedProviders = tmDbItem.Providers?.MapTmDbToItemProviders() ?? [];
        if (selectedProvider != null)
        {
            var updatedSelectedProvider = updatedProviders.FirstOrDefault(x => x.Id == selectedProvider.Id);
            if (updatedSelectedProvider != null) updatedSelectedProvider.Selected = true;
        }

        item.Name = tmDbItem.ItemName ?? item.Name;
        item.Type = tmDbItem.MediaType == "tv"
            ? ItemType.Tv
            : ItemType.Movie;
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
        item.LastUpdated = DateTimeOffset.UtcNow;
        item.SetImages(imageList.MapTmDbToImageList());
        item.SetProviders(updatedProviders);
        item.SetSeasons(tmDbItem.Seasons.MapTmDbToItemSeasons(seasons));
        item.SetCredits(tmDbItem.Credits.MapTmDbCreditsToItemCredits());
        if (string.IsNullOrWhiteSpace(item.BackdropUrl))
            item.BackdropUrl = tmDbItem.BackdropPath ?? imageList.Backdrops.FirstOrDefault()?.FilePath ?? string.Empty;
        //if (string.IsNullOrWhiteSpace(item.PosterUrl))
        item.PosterUrl = tmDbItem.PosterPath ?? imageList.Posters.FirstOrDefault()?.FilePath ?? string.Empty;
    }

    private static string Year(this string date)
    {
        if (string.IsNullOrWhiteSpace(date)) return "";
        var year = DateTime.Parse(date).Year.ToString();
        return year;
    }

    private static string GetNotes(this TmDbItem tmDbItem)
    {
        if (tmDbItem.MediaType != "tv")
        {
            var note = tmDbItem.ReleaseDate?.Year() ?? "";
            if (tmDbItem.OriginalLanguage != null && !tmDbItem.OriginalLanguage.Equals("en", StringComparison.OrdinalIgnoreCase))
                note += $" ({tmDbItem.OriginalLanguage.ToUpper()})";
            return $"{note}";
        }

        if (tmDbItem.NextEpisodeToAir == null)
        {
            if (tmDbItem.LastEpisodeToAir == null) return "";
            var then = DateTime.Parse(tmDbItem.LastEpisodeToAir.AirDate);
            return DateTime.Now.Subtract(then).TotalDays <= 90
                ? $"S{tmDbItem.LastEpisodeToAir.SeasonNumber} finale -> {HumanizeWithTodayOption(then)}"
                : $"{tmDbItem.FirstAirDate?.Year()} - {tmDbItem.LastAirDate?.Year()} ({tmDbItem.NumberOfSeasons} seasons)";
        }
        else
        {
            var then = DateTime.Parse(tmDbItem.NextEpisodeToAir.AirDate);
            return
                $"S{tmDbItem.NextEpisodeToAir.SeasonNumber} E{tmDbItem.NextEpisodeToAir.EpisodeNumber}/{EpisodeCount(tmDbItem.NextEpisodeToAir.SeasonNumber)} -> {HumanizeWithTodayOption(then)}";
        }

        string HumanizeWithTodayOption(DateTime d)
        {
            var result = d.Humanize();
            return result == "now" ? "today!" : result;
        }

        string EpisodeCount(int seasonNumber)
        {
            var season = tmDbItem.Seasons.FirstOrDefault(x => x.SeasonNumber == seasonNumber);
            return season?.EpisodeCount.ToString() ?? "?";
        }
    }

    private static ItemCredits MapTmDbCreditsToItemCredits(this TmDbCredits tmDbCredits)
    {
        return new ItemCredits
        {
            Cast = tmDbCredits.Cast.Select(x => new ItemCreditCastMember
            {
                Name = x.Name,
                ProfilePath = x.ProfilePath,
                Character = x.Character,
                Order = x.Order
            }).ToList(),
            Crew = tmDbCredits.Crew.Select(x => new ItemCreditCrewMember
            {
                Name = x.Name,
                ProfilePath = x.ProfilePath,
                Department = x.Department,
                Job = x.Job
            }).ToList()
        };
    }

    private static List<ItemSeason> MapTmDbToItemSeasons(this TmDbSeason[] tmDbItemSeasons, List<TmDbSeason> seasons)
    {
        return tmDbItemSeasons.Select(x => new ItemSeason
        {
            Name = x.Name,
            SeasonNumber = x.SeasonNumber,
            EpisodeCount = x.EpisodeCount,
            AirDate = x.AirDate,
            Overview = x.Overview,
            PosterPath = x.PosterPath,
            VoteAverage = x.VoteAverage,
            Episodes = seasons.FirstOrDefault(y => y.SeasonNumber == x.SeasonNumber)?.Episodes
                .OrderByDescending(y => y.EpisodeNumber)
                .Take(30)
                .Select(y => new ItemEpisode
                {
                    SeasonNumber = x.SeasonNumber,
                    EpisodeNumber = y.EpisodeNumber,
                    Name = y.Name,
                    Overview = y.Overview,
                    AirDate = y.AirDate,
                    StillPath = y.StillPath,
                    VoteAverage = y.VoteAverage,
                    VoteCount = y.VoteCount,
                    ProductionCode = y.ProductionCode,
                    Runtime = y.Runtime,
                    EpisodeType = y.EpisodeType,
                }).ToList() ?? []
        }).ToList();
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
        if (tmDbItem.Status.Equals("Canceled", StringComparison.OrdinalIgnoreCase)) return SeriesStatus.Canceled;
        if (tmDbItem.Status.Equals("Ended", StringComparison.OrdinalIgnoreCase)) return SeriesStatus.Ended;
        if (tmDbItem.Status.Equals("Returning Series", StringComparison.OrdinalIgnoreCase))
            return tmDbItem.NextEpisodeToAir == null ? SeriesStatus.Returning : SeriesStatus.InProgress;

        return null;
    }
}