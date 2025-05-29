using Microsoft.EntityFrameworkCore;
using WatchBoard.Database;
using WatchBoard.Database.Entities;
using WatchBoard.Services.TmDb;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services;

public interface IRepository
{
    Task<List<Board>> GetBoards();
    Task<Board?> GetBoard(Guid? id);

    Task<List?> GetList(Guid id);
    Task SortList(Guid id, string?[] itemIdsStr);

    Task<List<Item>> GetItems();
    Task<Item?> GetItem(Guid id);
    Task<Item> AddItemToBoard(Guid? boardId, int tmDbId, string type);
    Task MoveItemToOtherBoard(Guid itemId, Guid boardId);
    Task<Item> SetItemProvider(Guid itemId, int providerId);
    Task<Item> SetItemBackdrop(Guid itemId, Guid imageId);
    Task<string> GetItemBackdropUrl(Guid itemId, Guid imageId);
    Task<Item> RefreshItem(Guid itemId);
    Task DeleteItem(Guid id);
    Task<List<Item>> SearchForItems(string keyword, ItemType itemType);
}

public class Repository(AppDbContext db, ITmDb tmDb) : IRepository
{
    public async Task<Board?> GetBoard(Guid? id)
    {
        var boards = db.Boards
            .AsNoTracking()
            .Include(x => x.Lists.OrderByDescending(l => l.Order))
            .ThenInclude(x => x.Items.OrderBy(i => i.Order));
        if (id.HasValue)
            return await boards.FirstOrDefaultAsync(x => x.Id == id);
        return await boards.FirstOrDefaultAsync();
    }

    public async Task<List<Board>> GetBoards()
    {
        return await db.Boards
            .Include(x => x.Lists.OrderBy(l => l.Order))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List?> GetList(Guid id)
    {
        return await db.Lists
            .AsNoTracking()
            .Include(x => x.Items.OrderBy(y => y.Order))
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Item>> GetItems()
    {
        return db.Items
            .AsNoTracking().ToListAsync();
    }

    public async Task<Item?> GetItem(Guid id)
    {
        return await db.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Item> AddItemToBoard(Guid? boardId, int tmDbId, string type)
    {
        var listId = db.Lists.FirstOrDefault(x => x.Name == "Queue" && x.BoardId == boardId)?.Id
                     ??
                     (db.Boards
                         .Include(x => x.Lists.OrderByDescending(l => l.Order))
                         .FirstOrDefault(x => x.Id == boardId)?
                         .Lists.FirstOrDefault()?.Id ?? throw new KeyNotFoundException());

        var order = (db.Items.AsNoTracking().Where(x => x.ListId == listId).OrderByDescending(x => x.Order).FirstOrDefault()?.Order ?? -1) + 1;
        var dbItem = new Item
        {
            TmdbId = tmDbId,
            Type = type == "tv"
                ? ItemType.Tv
                : ItemType.Movie,
            ListId = listId,
            Order = order
        };
        await UpdateItemFromTmDb(dbItem);
        db.Items.Add(dbItem);
        await db.SaveChangesAsync();
        return dbItem;
    }

    public async Task<Item> RefreshItem(Guid itemId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        await UpdateItemFromTmDb(dbItem);
        await db.SaveChangesAsync();
        return dbItem;
    }
    
    public async Task MoveItemToOtherBoard(Guid itemId, Guid boardId)
    {
        var item = await db.Items.FindAsync(itemId);
        if (item == null) return;

        var itemList = await db.Lists.FirstOrDefaultAsync(x => x.Id == item.ListId);
        var otherLists = await db.Lists.Where(x => x.BoardId == boardId).ToListAsync();
        var newList = otherLists.FirstOrDefault(x => x.Name.Equals(itemList?.Name, StringComparison.OrdinalIgnoreCase)) ?? otherLists.FirstOrDefault();
        if (newList == null) return;

        item.ListId = newList.Id;
        await db.SaveChangesAsync();
    }

    public async Task<Item> SetItemProvider(Guid itemId, int providerId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        var providers = dbItem.GetProviders();
        foreach (var p in providers)
        {
            p.Selected = providerId == p.Id;
        }

        dbItem.SetProviders(providers);
        await db.SaveChangesAsync();
        return dbItem;
    }

    public async Task<Item> SetItemBackdrop(Guid itemId, Guid imageId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        var img = dbItem.GetBackdropImages().FirstOrDefault(x => x.Id == imageId) ?? throw new KeyNotFoundException();
        dbItem.BackdropBase64 = await tmDb.GetImageBase64(img.UrlPath);
        dbItem.BackdropUrl = img.UrlPath;
        await db.SaveChangesAsync();
        return dbItem;
    }

    public async Task<string> GetItemBackdropUrl(Guid itemId, Guid imageId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        var img = dbItem.GetBackdropImages().FirstOrDefault(x => x.Id == imageId) ?? throw new KeyNotFoundException();
        return await tmDb.GetImageUrl(img.UrlPath);
    }

    public async Task DeleteItem(Guid id)
    {
        var item = await db.Items.FindAsync(id);
        if (item == null) return;
        db.Remove(item);
        await db.SaveChangesAsync();
    }

    public async Task SortList(Guid id, string?[] itemIdsStr)
    {
        var dbItems = db.Items.Where(x => x.ListId == id).ToList();

        var newItemIds = itemIdsStr
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => Guid.TryParse(x, out var itemIdGuid) ? itemIdGuid : Guid.Empty);

        var itemPosition = 0;
        foreach (var itemId in newItemIds.Where(x => x != Guid.Empty))
        {
            var dbItem = dbItems.FirstOrDefault(x => x.Id == itemId);
            if (dbItem == null)
            {
                // new item wasn't in this list
                dbItem = db.Items.FirstOrDefault(x => x.Id == itemId);
                if (dbItem != null)
                {
                    // move it to this list
                    dbItem.Order = itemPosition;
                    dbItem.ListId = id;
                }
            }
            else
            {
                // just set the order
                dbItem.Order = itemPosition;
            }

            itemPosition++;
        }

        await db.SaveChangesAsync();
    }

    public async Task<List<Item>> SearchForItems(string keyword, ItemType itemType)
    {
        var tmDbResults = await tmDb.Search(keyword, itemType.ToString().ToLower());
        var items = tmDbResults.Select(x => new Item
        {
            Id = Guid.Empty,
            Name = x.ItemName ?? "UNKNOWN",
            Type = x.MediaType == "tv" ? ItemType.Tv : ItemType.Movie,
            TagLine = x.TagLine,
            ReleaseDate = x.ItemReleaseDate,
            EndDate = x.LastAirDate,
            NumberOfSeasons = x.NumberOfSeasons,
            TmdbId = x.Id,
            PosterUrl = x.PosterPath ?? "/img/ph.png",
            BackdropUrl = x.BackdropPath ?? "/img/ph.png",
            OriginalLanguage = x.OriginalLanguage?.ToUpper() ?? "",
            OriginCountry = string.Join(", ", x.OriginCountry)
        }).ToList();
        return items;
    }

    private async Task UpdateItemFromTmDb(Item dbItem)
    {
        var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
        var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());

        var latestSeasons = tmDbItem.Seasons
            .OrderByDescending(x => x.SeasonNumber)
            .Take(3);
        var tmDbItemSeasons = new List<TmDbSeason>();
        foreach (var item in latestSeasons)
        {
            var tmDbItemSeason = await tmDb.GetSeason(dbItem.TmdbId, item.SeasonNumber);
            tmDbItemSeasons.AddRange(tmDbItemSeason);
        }

        dbItem.UpdateFromTmDb(tmDbItem, images, tmDbItemSeasons.ToList());

        if (string.IsNullOrWhiteSpace(dbItem.BackdropBase64)) 
            dbItem.BackdropBase64 = await tmDb.GetImageBase64(dbItem.BackdropUrl);
        if (string.IsNullOrWhiteSpace(dbItem.PosterBase64)) 
            dbItem.PosterBase64 = await tmDb.GetImageBase64(dbItem.PosterUrl, "w185");
    }
}