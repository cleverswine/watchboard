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
        item.TmdbId = tmDbItem.Id;
        item.Type = tmDbItem.MediaType == "tv"
            ? ItemType.Tv
            : ItemType.Movie;
        item.Overview = tmDbItem.Overview;
        item.TagLine = tmDbItem.TagLine;
        item.ReleaseDate = tmDbItem.ItemReleaseDate;
        item.EndDate = tmDbItem.LastAirDate;
        item.NumberOfSeasons = tmDbItem.NumberOfSeasons;
        item.ImdbId = tmDbItem.ExternalIds.ImdbId;
        item.SeriesStatus = tmDbItem.MapTmDbToSeriesStatus();
        item.SeriesNextEpisodeDate = tmDbItem.NextEpisodeToAir?.AirDate;
        item.SeriesNextEpisodeNumber = tmDbItem.NextEpisodeToAir?.EpisodeNumber;
        item.SeriesNextEpisodeSeason = tmDbItem.NextEpisodeToAir?.SeasonNumber;
        item.OriginalLanguage = tmDbItem.OriginalLanguage?.ToUpper() ?? "";
        item.OriginCountry = string.Join(", ", tmDbItem.OriginCountry);
        item.Notes = tmDbItem.GetNotes();
        item.RunTime = tmDbItem.GetRunTime();
        item.LastUpdated = DateTimeOffset.UtcNow;
        item.SetProviders(updatedProviders);
        item.SetCredits(tmDbItem.Credits.MapTmDbCreditsToItemCredits());
        item.PosterUrl = tmDbItem.PosterPath ?? imageList.Posters.FirstOrDefault()?.FilePath ?? string.Empty;
        item.BackdropUrl = tmDbItem.BackdropPath ?? imageList.Backdrops.FirstOrDefault()?.FilePath ?? string.Empty;
    }

    public static string Year(this string date)
    {
        if (string.IsNullOrWhiteSpace(date)) return "";
        var year = DateTime.Parse(date).Year.ToString();
        return year;
    }

    private static int GetRunTime(this TmDbItem item)
    {
        if(item.RunTime != null) return item.RunTime.Value;
        if(item.NextEpisodeToAir?.Runtime != null) return item.NextEpisodeToAir.Runtime.Value;
        if(item.LastEpisodeToAir?.Runtime != null) return item.LastEpisodeToAir.Runtime.Value;
        return 0;
    }
    
    private static string GetNotes(this TmDbItem tmDbItem)
    {
        if (tmDbItem.MediaType != "tv") return "";

        if (tmDbItem.NextEpisodeToAir != null && !string.IsNullOrWhiteSpace(tmDbItem.NextEpisodeToAir.AirDate))
        {
            var then = DateOnly.Parse(tmDbItem.NextEpisodeToAir.AirDate);
            return $"S{tmDbItem.NextEpisodeToAir.SeasonNumber:00} E{tmDbItem.NextEpisodeToAir.EpisodeNumber:00} -> {HumanizeWithTodayOption(then)}";
        }
        else
        {
            if (tmDbItem.LastEpisodeToAir == null || string.IsNullOrWhiteSpace(tmDbItem.LastEpisodeToAir.AirDate)) return "";
            var then = DateOnly.Parse(tmDbItem.LastEpisodeToAir.AirDate);
            return DateTime.Now.Subtract(DateTime.Parse(tmDbItem.LastEpisodeToAir.AirDate)).TotalDays < 7 
                ? $"S{tmDbItem.LastEpisodeToAir.SeasonNumber} finale -> {HumanizeWithTodayOption(then)}" 
                : "";
        }

        string HumanizeWithTodayOption(DateOnly d)
        {
            var result = d.Humanize();
            return result == "now" ? "today!" : result;
        }

        int EpisodeCount(int seasonNumber)
        {
            var season = tmDbItem.Seasons.FirstOrDefault(x => x.SeasonNumber == seasonNumber);
            return season?.EpisodeCount ?? 0;
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
    
    private static SeriesStatus? MapTmDbToSeriesStatus(this TmDbItem tmDbItem)
    {
        if (tmDbItem.MediaType != "tv" || tmDbItem.Status == null) return null;
        
        if (tmDbItem.Status.Equals("Canceled", StringComparison.OrdinalIgnoreCase)) 
            return SeriesStatus.Canceled;
        if (tmDbItem.Status.Equals("Ended", StringComparison.OrdinalIgnoreCase)) 
            return SeriesStatus.Ended;
        if (tmDbItem.Status.Equals("In Production", StringComparison.OrdinalIgnoreCase)) 
            return SeriesStatus.InProduction;
        if (tmDbItem.Status.Equals("Returning Series", StringComparison.OrdinalIgnoreCase))
            return tmDbItem.NextEpisodeToAir == null 
                ? SeriesStatus.Returning 
                : SeriesStatus.InProgress;

        return SeriesStatus.Other;
    }
}