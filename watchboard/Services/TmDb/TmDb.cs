using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services.TmDb;

public interface ITmDb
{
    Task<List<TmdbItem>> Search(string query, int limit = 8);
    Task<TmdbItem> GetDetail(int id, string type);
    Task<ImageList> GetImages(int id, string type);
    Task<string> GetImageBase64(string imagePath, string size = "w154");
    Task<string> GetImageUrl(string imagePath, string size = "w154");
    Task<List<TmDbProvider>> GetProviders(string type, string region = "US");
}

public class TmDb : ITmDb
{
    private static readonly JsonSerializerOptions JsonOpts = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
    private static readonly string[] ItemMediaTypes = ["movie", "tv"];
    private static readonly string BaseApiPath = "https://api.themoviedb.org/3/";
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;

    public TmDb(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<List<TmDbProvider>> GetProviders(string type, string region = "US")
    {
        await Task.Yield();

        if (_cache.TryGetValue($"TmdbGetProviders-{type}-{region}", out List<TmDbProvider>? results) && results is not null)
            return results;

        //var url = $"{BaseApiPath}watch/providers/{type}?watch_region={region}";
        var json = await File.ReadAllTextAsync("Services/Json/movieproviders.json");
        results = (JsonSerializer.Deserialize<TmDbProviders>(json)?.Results ?? [])
            .Where(x => x.DisplayPriorities.ContainsKey("US")).ToList();
        _cache.Set($"TmdbGetProviders-{type}-{region}", results, TimeSpan.FromMinutes(60));
        return results;
    }

    public async Task<List<TmdbItem>> Search(string query, int limit = 8)
    {
        if (_cache.TryGetValue($"TmdbSearch-{query}-{limit}", out List<TmdbItem>? results) && results is not null)
            return results;

        // var json = await File.ReadAllTextAsync("Services/Json/search.json");
        // var searchResults = JsonSerializer.Deserialize<SearchResults>(json, JsonOpts);

        var queryUrlEncoded = HttpUtility.UrlEncode(query);
        var url = $"{BaseApiPath}search/multi?query={queryUrlEncoded}&include_adult=false&language=en-US&page=1";
        var searchResults = await _httpClient.GetFromJsonAsync<SearchResults>(url, JsonOpts);

        results = searchResults?.Results.Where(x => ItemMediaTypes.Contains(x.MediaType)).Take(limit).ToList();

        if (results != null && results.Count != 0)
            _cache.Set($"TmdbSearch-{query}-{limit}", searchResults, TimeSpan.FromMinutes(5));

        var configuration = await GetConfiguration();
        foreach (var result in results ?? [])
        {
            result.PosterPath = configuration.Images.BaseUrl + "w92" + result.PosterPath;
            if (!string.IsNullOrWhiteSpace(result.BackdropPath))
                result.BackdropPath = configuration.Images.BaseUrl + "w300" + result.BackdropPath;
            else
                result.BackdropPath = configuration.Images.BaseUrl + "w185" + result.PosterPath;
        }

        return results ?? [];
    }

    public async Task<TmdbItem> GetDetail(int id, string type)
    {
        if (_cache.TryGetValue($"TmdbDetail-{type}-{id}", out TmdbItem? item) && item is not null)
            return item;

        var url = $"{BaseApiPath}{type.ToLower()}/{id}?append_to_response=latest%2Cexternal_ids%2Cwatch%2Fproviders&language=en-US";
        item = await _httpClient.GetFromJsonAsync<TmdbItem>(url, JsonOpts);
        if (item == null) throw new NullReferenceException("TmDb Item is null");
        item.MediaType = type;

        _cache.Set($"TmdbDetail-{type}-{id}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<ImageList> GetImages(int id, string type)
    {
        if (_cache.TryGetValue($"TmdbImages-{type}-{id}", out ImageList? item) && item is not null)
            return item;

        var url = $"{BaseApiPath}{type.ToLower()}/{id}/images";
        item = await _httpClient.GetFromJsonAsync<ImageList>(url, JsonOpts);
        if (item == null) throw new NullReferenceException("TmDb Item is null");

        _cache.Set($"TmdbImages-{type}-{id}", item, TimeSpan.FromMinutes(60));
        return item;
    }

    public async Task<string> GetImageUrl(string imagePath, string size = "w154")
    {
        ArgumentNullException.ThrowIfNull(imagePath);
        var configuration = await GetConfiguration();
        return configuration.Images.BaseUrl + size + imagePath;
    }

    public async Task<string> GetImageBase64(string imagePath, string size = "w154")
    {
        ArgumentNullException.ThrowIfNull(imagePath);

        if (_cache.TryGetValue($"GetImageBase64-{imagePath}-{size}", out string? data) && data is not null)
            return data;

        var url = await GetImageUrl(imagePath, size);
        var b = await _httpClient.GetByteArrayAsync(url);

        var imageExtension = imagePath.Split(".").Last();
        data = $"data:image/{imageExtension};base64,{Convert.ToBase64String(b)}";
        _cache.Set($"TmdbPoster-{imagePath}", data, TimeSpan.FromMinutes(1));
        return data;
    }

    private async Task<TmdbConfiguration> GetConfiguration()
    {
        if (_cache.TryGetValue("TmdbConfiguration", out TmdbConfiguration? configuration) && configuration is not null)
            return configuration;

        configuration = await _httpClient.GetFromJsonAsync<TmdbConfiguration>($"{BaseApiPath}configuration", JsonOpts);
        _cache.Set("TmdbConfiguration", configuration, TimeSpan.FromMinutes(60));
        return configuration ?? new TmdbConfiguration();
    }
}