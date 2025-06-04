using System.Web;
using Microsoft.Extensions.Caching.Memory;
using WatchBoard.TmDb.Models;

namespace WatchBoard.TmDb;

public interface ITmDbService
{
    Task<SearchResult> Search(string query);
    Task<TvShow> GetTvShow(int id);
    Task<Movie> GetMovie(int id);
    Task<Configuration> GetConfiguration();
    Task<(string ImageType, byte[] Data)> GetImage(string filePath, string size);
}

public class TmDbService(ILogger<ITmDbService> logger, HttpClient httpClient, IMemoryCache cache) : ITmDbService
{
    private static readonly string BaseApiPath = "https://api.themoviedb.org/3";

    public async Task<SearchResult> Search(string query)
    {
        var queryUrlEncoded = HttpUtility.UrlEncode(query);
        var url = $"{BaseApiPath}/search/multi?query={queryUrlEncoded}&include_adult=false&language=en-US&page=1";
        return await GetOrCreate(url, async () =>
            await httpClient.GetFromJsonAsync<SearchResult>(url) ?? throw new HttpRequestException("No search result"));
    }

    public async Task<TvShow> GetTvShow(int id)
    {
        var url = $"{BaseApiPath}/tv/{id}?append_to_response=credits%2Cexternal_ids%2Cwatch%2Fproviders%2Clatest&language=en-US";
        return await GetOrCreate(url, async () =>
        {
            var result = await httpClient.GetFromJsonAsync<TvShow>(url) ?? throw new HttpRequestException("No TV show found");
            url = $"{BaseApiPath}/tv/{id}/images?language=en";
            var images = await httpClient.GetFromJsonAsync<ImagesItem>(url) ?? throw new HttpRequestException("No TV show images found");
            result.Images = images;
            return result;
        });
    }

    public async Task<Movie> GetMovie(int id)
    {
        var url = $"{BaseApiPath}/movie/{id}?append_to_response=credits%2Cexternal_ids%2Crelease_dates%2Cwatch%2Fproviders&language=en-US";
        return await GetOrCreate(url, async () =>
        {
            var result = await httpClient.GetFromJsonAsync<Movie>(url) ?? throw new HttpRequestException("No Movie found");
            url = $"{BaseApiPath}/movie/{id}/images?language=en";
            var images = await httpClient.GetFromJsonAsync<ImagesItem>(url) ?? throw new HttpRequestException("No Movie images found");
            result.Images = images;
            return result;
        });
    }

    public async Task<Configuration> GetConfiguration()
    {
        var url = $"{BaseApiPath}/configuration";
        return await GetOrCreate(url, async () =>
            await httpClient.GetFromJsonAsync<Configuration>(url) ?? throw new HttpRequestException("No configuration found"), 300);
    }

    public async Task<(string ImageType, byte[] Data)> GetImage(string filePath, string size)
    {
        var configuration = await GetConfiguration();
        var url = $"{configuration.Images.BaseUrl}{size}{filePath}";
        return await GetOrCreate(url, async () =>
            ($"image/{filePath.Split(".").Last()}", await httpClient.GetByteArrayAsync(url)));
    }

    private async Task<T> GetOrCreate<T>(string url, Func<Task<T>> factory, int cacheMinutes = 60)
    {
        if (cache.TryGetValue(url, out T? cached)) return cached ?? throw new Exception("Cached item is null");
        logger.LogInformation($"fetching {url}");
        var result = await factory();
        cache.Set(url, result, TimeSpan.FromMinutes(cacheMinutes));
        return result;
    }
}