using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services.TmDb;

public interface ITmDb
{
    Task<List<TmDbItem>> Search(string query, string type = "tv", int limit = 8);
    Task<TmDbItem> GetDetail(int id, string type);
    Task<TmDbItem> GetDetailByImDbId(string id);
    Task<TmDbImages> GetImages(int id, string type);
    Task<TmDbSeason> GetSeason(int id, int seasonNumber);
    Task<string?> GetImageBase64(string imagePath, string size = "w300");
}

public class TmDb(HttpClient httpClient, IMemoryCache cache) : ITmDb
{
    private static readonly JsonSerializerOptions JsonOpts = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
    private static readonly string BaseApiPath = "https://api.themoviedb.org/3/";

    
    /// <summary>
    /// Fetches the detailed item JSON from TMDB for the given id and type ("tv" or "movie").
    /// This method will:
    /// - fetch the base detail with appended responses (latest, external_ids, credits, watch/providers)
    /// - fetch and attach the images JSON under the "images" property
    /// - for each season in the returned "seasons" array, fetch the season detail and attach it
    ///   under the season object's "detail" property. If the season detail contains an "episodes"
    ///   array, it will also attach that array directly under the season as "episodes".
    /// The combined JsonNode is cached for 60 minutes under the key "TmDbDetailJson-{type}-{id}".
    /// </summary>
    public async Task<JsonNode> GetDetailJson(int id, string type)
    {
        var url = $"{BaseApiPath}{type.ToLower()}/{id}?append_to_response=latest%2Cexternal_ids%2Ccredits%2Cwatch%2Fproviders&language=en-US";

        // get the main detail as JsonNode so we can mutate it
        var respStream = await httpClient.GetStreamAsync(url);
        using var doc = await JsonDocument.ParseAsync(respStream);
        var root = JsonNode.Parse(doc.RootElement.GetRawText()) ?? new JsonObject();

        var mainObj = root as JsonObject ?? new JsonObject();
        
        // add media type
        mainObj["media_type"] = type;
        
        // prune watch/providers results to only include US
        try
        {
            if (mainObj["watch/providers"] is JsonObject wp && wp["results"] is JsonObject results)
            {
                // collect keys to remove (all except "US")
                var keysToRemove = results.Select(kvp => kvp.Key).Where(k => !string.Equals(k, "US", StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var k in keysToRemove)
                {
                    results.Remove(k);
                }
            }
        }
        catch (Exception)
        {
            // non-fatal: ignore prune errors
        }
        
        // fetch images and attach
        try
        {
            var imagesUrl = $"{BaseApiPath}{type.ToLower()}/{id}/images";
            var imagesStream = await httpClient.GetStreamAsync(imagesUrl);
            using var imagesDoc = await JsonDocument.ParseAsync(imagesStream);
            var imagesNode = JsonNode.Parse(imagesDoc.RootElement.GetRawText()) ?? new JsonObject();
            mainObj["images"] = imagesNode;
        }
        catch (Exception)
        {
            // ignore image fetch failure
        }

        // iterate seasons if present and fetch each season details
        // if (mainObj["seasons"] is JsonArray seasons)
        // {
        //     var seasonTasks = new List<Task>();
        //     for (var i = 0; i < seasons.Count; i++)
        //     {
        //         if (seasons[i] is JsonObject sObj)
        //         {
        //             var snText = sObj["season_number"]?.ToString();
        //             if (!string.IsNullOrWhiteSpace(snText) && int.TryParse(snText, out var seasonNumber))
        //             {
        //                 var idx = i;
        //                 var sn = seasonNumber;
        //                 seasonTasks.Add(Task.Run(async () =>
        //                 {
        //                     try
        //                     {
        //                         var seasonUrl = $"{BaseApiPath}tv/{id}/season/{sn}?append_to_response=watch%2Fproviders&language=en-US";
        //                         var sStream = await httpClient.GetStreamAsync(seasonUrl);
        //                         using var sDoc = await JsonDocument.ParseAsync(sStream);
        //                         var seasonDetailNode = JsonNode.Parse(sDoc.RootElement.GetRawText()) ?? new JsonObject();
        //                         // attach under a property 'detail' on the season object
        //                         lock (seasons)
        //                         {
        //                             if (seasons[idx] is JsonObject target)
        //                             {
        //                                 target["detail"] = seasonDetailNode;
        //                                 // if season detail contains episodes, attach them directly for quick access
        //                                 if (seasonDetailNode is JsonObject sdObj && sdObj["episodes"] is JsonArray eps)
        //                                 {
        //                                     target["episodes"] = eps;
        //                                 }
        //                             }
        //                         }
        //                     }
        //                     catch (Exception)
        //                     {
        //                         // ignore per-season fetch errors
        //                     }
        //                 }));
        //             }
        //         }
        //     }
        //
        //     await Task.WhenAll(seasonTasks);
        // }
        
        return root;
    }
    
    public async Task<List<TmDbItem>> Search(string query, string type = "tv", int limit = 8)
    {
        if (cache.TryGetValue($"TmDbSearch-{query}-{limit}-{type}", out List<TmDbItem>? results) && results is not null)
            return results;

        var queryUrlEncoded = HttpUtility.UrlEncode(query);
        var url = $"{BaseApiPath}search/{type}?query={queryUrlEncoded}&include_adult=false&language=en-US&page=1";
        var searchResults = await httpClient.GetFromJsonAsync<TmDbSearchResults>(url, JsonOpts);

        results = searchResults?.Results.Take(limit).ToList();

        if (results != null && results.Count != 0)
            cache.Set($"TmDbSearch-{query}-{limit}-{type}", searchResults, TimeSpan.FromMinutes(5));

        var configuration = await GetConfiguration();
        foreach (var result in results ?? [])
        {
            result.MediaType = type;
            result.PosterPath = configuration.Images.BaseUrl + "w92" + result.PosterPath;
            if (!string.IsNullOrWhiteSpace(result.BackdropPath))
                result.BackdropPath = configuration.Images.BaseUrl + "w300" + result.BackdropPath;
            else
                result.BackdropPath = configuration.Images.BaseUrl + "w185" + result.PosterPath;
        }

        return results ?? [];
    }

    public async Task<TmDbItem> GetDetail(int id, string type)
    {
        if (cache.TryGetValue($"TmDbDetail-{type}-{id}", out TmDbItem? item) && item is not null)
            return item;

        var url = $"{BaseApiPath}{type.ToLower()}/{id}?append_to_response=latest%2Cexternal_ids%2Ccredits%2Cwatch%2Fproviders&language=en-US";
        item = await httpClient.GetFromJsonAsync<TmDbItem>(url, JsonOpts);
        if (item == null) throw new NullReferenceException("TmDb Item is null");
        item.MediaType = type;

        cache.Set($"TmDbDetail-{type}-{id}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<TmDbItem> GetDetailByImDbId(string id)
    {
        if (cache.TryGetValue($"GetDetailByImDbId-{id}", out TmDbItem? item) && item is not null)
            return item;

        var url = $"{BaseApiPath}find/{id}?external_source=imdb_id";
        var items = await httpClient.GetFromJsonAsync<TmDbByIdResults>(url, JsonOpts);
        if (items == null) throw new NullReferenceException("TmDb Item by ID is null");

        item = items.TvResults.FirstOrDefault() ?? items.MovieResults.FirstOrDefault() ?? throw new NullReferenceException("TmDb Item by ID is null");
        var configuration = await GetConfiguration();
        item.PosterPath = configuration.Images.BaseUrl + "w92" + item.PosterPath;

        cache.Set($"GetDetailByImDbId-{id}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<TmDbImages> GetImages(int id, string type)
    {
        if (cache.TryGetValue($"TmDbImages-{type}-{id}", out TmDbImages? item) && item is not null)
            return item;

        var url = $"{BaseApiPath}{type.ToLower()}/{id}/images";
        item = await httpClient.GetFromJsonAsync<TmDbImages>(url, JsonOpts);
        if (item == null) throw new NullReferenceException("TmDb Item is null");

        cache.Set($"TmDbImages-{type}-{id}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<TmDbSeason> GetSeason(int id, int seasonNumber)
    {
        if (cache.TryGetValue($"TmDbSeason-{id}-{seasonNumber}", out TmDbSeason? item) && item is not null)
            return item;

        var url = $"{BaseApiPath}tv/{id}/season/{seasonNumber}?append_to_response=watch%2Fproviders&language=en-US";
        item = await httpClient.GetFromJsonAsync<TmDbSeason>(url, JsonOpts);
        if (item == null) throw new NullReferenceException("TmDb Item is null");

        cache.Set($"TmDbSeason-{id}-{seasonNumber}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<string?> GetImageBase64(string imagePath, string size = "w300")
    {
        ArgumentNullException.ThrowIfNull(imagePath);

        if (cache.TryGetValue($"GetImageBase64-{imagePath}-{size}", out string? data) && data is not null)
            return data;

        var url = await GetImageUrl(imagePath, size);
        try
        {
            var b = await httpClient.GetByteArrayAsync(url);
            var imageExtension = imagePath.Split(".").Last();
            data = $"data:image/{imageExtension};base64,{Convert.ToBase64String(b)}";
            cache.Set($"GetImageBase64-{imagePath}-{size}", data, TimeSpan.FromMinutes(1));
            return data;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<string> GetImageUrl(string imagePath, string size = "w300")
    {
        ArgumentNullException.ThrowIfNull(imagePath);
        var configuration = await GetConfiguration();
        return configuration.Images.BaseUrl + size + imagePath;
    }
    
    private async Task<TmDbConfiguration> GetConfiguration()
    {
        if (cache.TryGetValue("TmdDConfiguration", out TmDbConfiguration? configuration) && configuration is not null)
            return configuration;

        configuration = await httpClient.GetFromJsonAsync<TmDbConfiguration>($"{BaseApiPath}configuration", JsonOpts) ?? new TmDbConfiguration();
        configuration.Languages = await httpClient.GetFromJsonAsync<List<TmDbConfigurationLanguage>>($"{BaseApiPath}configuration/languages", JsonOpts) ?? [];
        configuration.Countries = await httpClient.GetFromJsonAsync<List<TmDbConfigurationCountry>>($"{BaseApiPath}configuration/countries", JsonOpts) ?? [];

        cache.Set("TmdDConfiguration", configuration, TimeSpan.FromMinutes(120));
        return configuration;
    }
}