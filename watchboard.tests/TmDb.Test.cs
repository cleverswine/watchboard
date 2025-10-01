using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using WatchBoard.Services.TmDb;

namespace watchboard.tests;

[TestFixture]
public class TmDbTest
{
    [Test]
    public async Task TestRaw()
    {
        int[] ids =
        [
            254013, 96580, 106379, 241609, 108255, 43982, 99494, 196268, 135157, 261980,
            79340, 87542, 279283, 87689, 226749, 88384, 103516, 82549, 230448, 258025,
            129888, 247767, 239826, 85948, 119051, 126660, 51976, 241092, 237347, 228079,
            107113, 156644, 64518, 213344, 243248, 250988, 273247, 95480, 69272, 41132,
            123403, 117376, 136658, 51828, 62474, 51823, 225780, 253977, 136311, 133727,
            153911, 130842, 250988, 137140
        ];

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer",
                "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1NTMwYTc4ZTJhMjlmYzg3ZWU3MjllZmE3NGI3MzMyYSIsIm5iZiI6MTcyNzM4NDAwNC41NjAxMzQsInN1YiI6IjY2ZTRiNDA1NDcxNmM5NDFiNWVlNmI2NCIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.uCwPV56UBl5XiK0VA8Ap-TbzuzyRpwzD_9wAi8gUqTY");
        var service = new TmDb(client, new MemoryCache(new MemoryCacheOptions()));
        var type = "tv";

        var ja = new JsonArray();
        foreach (var i in ids)
        {
            var result = await service.GetDetailJson(i, type);
            ja.Add(result);
        }

        await File.WriteAllTextAsync("/Users/Kevin.Noone/Code/knoone/watchboard/watchboard/Services/TmDb/Json/ItemAllDetails.json", ja.ToJsonString());
    }
}