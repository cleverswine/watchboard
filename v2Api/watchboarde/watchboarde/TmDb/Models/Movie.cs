using System.Text.Json.Serialization;

namespace WatchBoard.TmDb.Models;

[Serializable]
public class Movie : TmDbItemBase
{
    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    [JsonPropertyName("belongs_to_collection")]
    public MovieCollection BelongsToCollection { get; set; } = new();

    [JsonPropertyName("budget")]
    public int Budget { get; set; }

    [JsonPropertyName("genres")]
    public List<MovieGenre> Genres { get; set; } = [];

    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("origin_country")]
    public List<string> OriginCountry { get; set; } = [];

    [JsonPropertyName("original_language")]
    public string? OriginalLanguage { get; set; }

    [JsonPropertyName("original_title")]
    public string? OriginalTitle { get; set; }

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("production_companies")]
    public List<MovieProductionCompany> ProductionCompanies { get; set; } = [];

    [JsonPropertyName("production_countries")]
    public List<MovieProductionCountry> ProductionCountries { get; set; } = [];

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("revenue")]
    public long Revenue { get; set; }

    [JsonPropertyName("runtime")]
    public int Runtime { get; set; }

    [JsonPropertyName("spoken_languages")]
    public List<MovieSpokenLanguage> SpokenLanguages { get; set; } = [];

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("tagline")]
    public string? Tagline { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("video")]
    public bool Video { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }

    [JsonPropertyName("credits")]
    public MovieCredits Credits { get; set; } = new();

    [JsonPropertyName("external_ids")]
    public MovieExternalIds ExternalIds { get; set; } = new();

    [JsonPropertyName("watch/providers")]
    public MovieWatchProviders WatchProviders { get; set; } = new();
    
    public ImagesItem Images { get; set; } = new();
}

[Serializable]
public class MovieCollection
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }
}

[Serializable]
public class MovieGenre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

[Serializable]
public class MovieProductionCompany
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
public class MovieProductionCountry
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("iso_3166_1")]
    public string Iso3166_1 { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

[Serializable]
public class MovieSpokenLanguage
{
    [JsonPropertyName("english_name")]
    public string EnglishName { get; set; } = string.Empty;

    [JsonPropertyName("iso_639_1")]
    public string Iso639_1 { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

[Serializable]
public class MovieCredits
{
    [JsonPropertyName("cast")]
    public List<MovieCastMember> Cast { get; set; } = new();

    [JsonPropertyName("crew")]
    public List<MovieCrewMember> Crew { get; set; } = new();
}

[Serializable]
public class MovieCastMember
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

    [JsonPropertyName("cast_id")]
    public int? CastId { get; set; }

    [JsonPropertyName("character")]
    public string? Character { get; set; }

    [JsonPropertyName("credit_id")]
    public string? CreditId { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

[Serializable]
public class MovieCrewMember
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
public class MovieExternalIds
{
    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }

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
public class MovieWatchProviders
{
    [JsonPropertyName("results")]
    public MovieWatchProvidersResults Results { get; set; } = new();
}

[Serializable]
public class MovieWatchProvidersResults
{
    [JsonPropertyName("US")]
    public MovieWatchProvidersResultsUs Us { get; set; } = new();
}

[Serializable]
public class MovieWatchProvidersResultsUs
{
    public string? Link { get; set; }

    [JsonPropertyName("flatrate")]
    public MovieWatchProvidersResultsFlatRate[] FlatRate { get; set; } = [];
}

[Serializable]
public class MovieWatchProvidersResultsFlatRate
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