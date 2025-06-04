using System.Text.Json.Serialization;

namespace WatchBoard.TmDb.Models;

[Serializable]
public class TvShow : TmDbItemBase
{
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    [JsonPropertyName("created_by")]
    public List<TvShowCreatedBy> CreatedBy { get; set; } = [];

    [JsonPropertyName("episode_run_time")]
    public List<int> EpisodeRunTime { get; set; } = [];

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; }

    [JsonPropertyName("genres")]
    public List<TvShowGenre> Genres { get; set; } = [];

    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    [JsonPropertyName("in_production")]
    public bool InProduction { get; set; }

    [JsonPropertyName("languages")]
    public List<string> Languages { get; set; } = [];

    [JsonPropertyName("last_air_date")]
    public string? LastAirDate { get; set; }

    [JsonPropertyName("last_episode_to_air")]
    public TvShowEpisode? LastEpisodeToAir { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("next_episode_to_air")]
    public TvShowEpisode? NextEpisodeToAir { get; set; }

    [JsonPropertyName("networks")]
    public List<TvShowNetwork> Networks { get; set; } = [];

    [JsonPropertyName("number_of_episodes")]
    public int NumberOfEpisodes { get; set; }

    [JsonPropertyName("number_of_seasons")]
    public int NumberOfSeasons { get; set; }

    [JsonPropertyName("origin_country")]
    public List<string> OriginCountry { get; set; } = [];

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string OriginalName { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("production_companies")]
    public List<TvShowProductionCompany> ProductionCompanies { get; set; } = [];

    [JsonPropertyName("production_countries")]
    public List<TvShowProductionCountry> ProductionCountries { get; set; } = [];

    [JsonPropertyName("seasons")]
    public List<TvShowSeason> Seasons { get; set; } = [];

    [JsonPropertyName("spoken_languages")]
    public List<TvShowSpokenLanguage> SpokenLanguages { get; set; } = [];

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("tagline")]
    public string Tagline { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }

    [JsonPropertyName("credits")]
    public TvShowCredits Credits { get; set; } = new();

    [JsonPropertyName("external_ids")]
    public TvShowExternalIds ExternalIds { get; set; } = new();

    [JsonPropertyName("watch/providers")]
    public TvShowWatchProviders WatchProviders { get; set; } = new();

    public ImagesItem Images { get; set; } = new();
}

[Serializable]
public class TvShowCreatedBy
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("credit_id")]
    public string? CreditId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string OriginalName { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public int Gender { get; set; }

    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; set; }
}

[Serializable]
public class TvShowGenre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

[Serializable]
public class TvShowEpisode
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }

    [JsonPropertyName("air_date")]
    public string? AirDate { get; set; }

    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("episode_type")]
    public string? EpisodeType { get; set; }

    [JsonPropertyName("production_code")]
    public string? ProductionCode { get; set; }

    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("show_id")]
    public int ShowId { get; set; }

    [JsonPropertyName("still_path")]
    public string? StillPath { get; set; }
}

[Serializable]
public class TvShowNetwork
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("origin_country")]
    public string OriginCountry { get; set; } = string.Empty;
}

[Serializable]
public class TvShowProductionCompany
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("origin_country")]
    public string OriginCountry { get; set; } = string.Empty;
}

[Serializable]
public class TvShowProductionCountry
{
    [JsonPropertyName("iso_3166_1")]
    public string Iso3166_1 { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

[Serializable]
public class TvShowSeason
{
    [JsonPropertyName("air_date")]
    public string? AirDate { get; set; }

    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }
}

[Serializable]
public class TvShowSpokenLanguage
{
    [JsonPropertyName("english_name")]
    public string EnglishName { get; set; } = string.Empty;

    [JsonPropertyName("iso_639_1")]
    public string Iso639_1 { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

[Serializable]
public class TvShowCredits
{
    [JsonPropertyName("cast")]
    public List<TvShowCastMember> Cast { get; set; } = new();

    [JsonPropertyName("crew")]
    public List<TvShowCrewMember> Crew { get; set; } = new();
}

[Serializable]
public class TvShowCastMember
{
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    [JsonPropertyName("gender")]
    public int Gender { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("known_for_department")]
    public string? KnownForDepartment { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string? OriginalName { get; set; }

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; set; }

    [JsonPropertyName("character")]
    public string? Character { get; set; }

    [JsonPropertyName("credit_id")]
    public string? CreditId { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

[Serializable]
public class TvShowCrewMember
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    [JsonPropertyName("gender")]
    public int Gender { get; set; }

    [JsonPropertyName("known_for_department")]
    public string? KnownForDepartment { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string? OriginalName { get; set; }

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; set; }

    [JsonPropertyName("credit_id")]
    public string? CreditId { get; set; }

    [JsonPropertyName("department")]
    public string? Department { get; set; }

    [JsonPropertyName("job")]
    public string? Job { get; set; }
}

[Serializable]
public class TvShowExternalIds
{
    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("freebase_mid")]
    public string? FreebaseMid { get; set; }

    [JsonPropertyName("freebase_id")]
    public string? FreebaseId { get; set; }

    [JsonPropertyName("tvdb_id")]
    public int? TvdbId { get; set; }

    [JsonPropertyName("tvrage_id")]
    public int? TvrageId { get; set; }

    [JsonPropertyName("wikidata_id")]
    public string? WikidataId { get; set; }

    [JsonPropertyName("facebook_id")]
    public string? FacebookId { get; set; }

    [JsonPropertyName("instagram_id")]
    public string? InstagramId { get; set; }

    [JsonPropertyName("twitter_id")]
    public string? TwitterId { get; set; }
}

[Serializable]
public class TvShowWatchProviders
{
    [JsonPropertyName("results")]
    public TvShowWatchProvidersResults Results { get; set; } = new();
}

[Serializable]
public class TvShowWatchProvidersResults
{
    [JsonPropertyName("US")]
    public TvShowWatchProvidersResultsUs Us { get; set; } = new();
}

[Serializable]
public class TvShowWatchProvidersResultsUs
{
    public string? Link { get; set; }

    [JsonPropertyName("flatrate")]
    public TvShowWatchProvidersResultsFlatRate[] FlatRate { get; set; } = [];
}

[Serializable]
public class TvShowWatchProvidersResultsFlatRate
{
    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; set; }

    [JsonPropertyName("provider_id")]
    public int ProviderId { get; set; }

    [JsonPropertyName("provider_name")]
    public string? ProviderName { get; set; }

    [JsonPropertyName("display_priority")]
    public int DisplayPriority { get; set; }
}