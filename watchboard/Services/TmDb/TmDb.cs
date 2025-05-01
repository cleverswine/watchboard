using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services.TmDb;

public interface ITmDb
{
    Task<List<TmDbItem>> Search(string query, string type = "tv", int limit = 8);
    Task<TmDbItem> GetDetail(int id, string type);
    Task<TmDbItemImageList> GetImages(int id, string type);
    Task<string> GetImageBase64(string imagePath, string size = "w300");
    Task<string> GetImageUrl(string imagePath, string size = "w300");
}

public class TmDb(HttpClient httpClient, IMemoryCache cache) : ITmDb
{
    private static readonly JsonSerializerOptions JsonOpts = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
    private static readonly string[] ItemMediaTypes = ["movie", "tv"];
    private static readonly string BaseApiPath = "https://api.themoviedb.org/3/";

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

        var url = $"{BaseApiPath}{type.ToLower()}/{id}?append_to_response=latest%2Cexternal_ids%2Cwatch%2Fproviders&language=en-US";
        item = await httpClient.GetFromJsonAsync<TmDbItem>(url, JsonOpts);
        if (item == null) throw new NullReferenceException("TmDb Item is null");
        item.MediaType = type;

        cache.Set($"TmDbDetail-{type}-{id}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<TmDbItemImageList> GetImages(int id, string type)
    {
        if (cache.TryGetValue($"TmDbImages-{type}-{id}", out TmDbItemImageList? item) && item is not null)
            return item;

        var url = $"{BaseApiPath}{type.ToLower()}/{id}/images";
        item = await httpClient.GetFromJsonAsync<TmDbItemImageList>(url, JsonOpts);
        if (item == null) throw new NullReferenceException("TmDb Item is null");

        cache.Set($"TmDbImages-{type}-{id}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<string> GetImageUrl(string imagePath, string size = "w300")
    {
        ArgumentNullException.ThrowIfNull(imagePath);
        var configuration = await GetConfiguration();
        return configuration.Images.BaseUrl + size + imagePath;
    }

    public async Task<string> GetImageBase64(string imagePath, string size = "w300")
    {
        ArgumentNullException.ThrowIfNull(imagePath);

        if (cache.TryGetValue($"GetImageBase64-{imagePath}-{size}", out string? data) && data is not null)
            return data;

        var url = await GetImageUrl(imagePath, size);
        var b = await httpClient.GetByteArrayAsync(url);

        var imageExtension = imagePath.Split(".").Last();
        data = $"data:image/{imageExtension};base64,{Convert.ToBase64String(b)}";
        cache.Set($"GetImageBase64-{imagePath}-{size}", data, TimeSpan.FromMinutes(1));
        return data;
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