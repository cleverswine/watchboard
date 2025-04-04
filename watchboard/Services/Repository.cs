using Microsoft.EntityFrameworkCore;
using WatchBoard.Services.Database;
using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.TmDb;

namespace WatchBoard.Services;

public interface IRepository
{
    Task<Board?> GetBoardById(Guid? id);
    Task<List<Board>> GetBoards();
    Task<List?> GetListById(Guid id);
    Task<Item?> GetItemById(Guid id);
    Task<Item> AddItemToBoard(Guid? boardId, int tmDbId, string type);
    Task<Item> AddItem(Guid listId, int tmDbId, string type);
    Task MoveItem(Guid itemId, Guid boardId);
    Task<Item> SetProvider(Guid itemId, string providerName);
    Task<Item> SetBackdrop(Guid itemId, Guid imageId);
    Task<Item> RefreshItem(Guid itemId);
    Task DeleteItem(Guid id);
    Task SortList(Guid id, string?[] itemIdsStr);
    Task<List<Item>> Search(string keyword);
}

public class Repository(AppDbContext db, ITmDb tmDb) : IRepository
{
    public async Task<Board?> GetBoardById(Guid? id)
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
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List?> GetListById(Guid id)
    {
        return await db.Lists
            .AsNoTracking()
            .Include(x => x.Items.OrderBy(y => y.Order))
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Item?> GetItemById(Guid id)
    {
        return await db.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Item> AddItemToBoard(Guid? boardId, int tmDbId, string type)
    {
        var listId = db.Boards
            .Include(x => x.Lists.OrderByDescending(l => l.Order))
            .FirstOrDefault(x => x.Id == boardId)?
            .Lists.FirstOrDefault()?.Id ?? throw new KeyNotFoundException();

        return await AddItem(listId, tmDbId, type);
    }
    
    public async Task<Item> AddItem(Guid listId, int tmDbId, string type)
    {
        var order = (db.Items.AsNoTracking().Where(x => x.ListId == listId).OrderByDescending(x => x.Order).FirstOrDefault()?.Order ?? -1) + 1;
        var tmDbItem = await tmDb.GetDetail(tmDbId, type);
        var images = await tmDb.GetImages(tmDbId, type);
        var dbItem = tmDbItem.MapFromTmDb(listId, images);
        dbItem.BackdropBase64 = await tmDb.GetImageBase64(dbItem.BackdropUrl, "w300");
        dbItem.PosterBase64 = await tmDb.GetImageBase64(dbItem.PosterUrl, "w92");
        dbItem.Order = order;
        db.Items.Add(dbItem);
        await db.SaveChangesAsync();
        return dbItem;
    }

    public async Task MoveItem(Guid itemId, Guid boardId)
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

    public async Task<Item> SetProvider(Guid itemId, string providerName)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        dbItem.SelectedProviderName = providerName;
        dbItem.Expanded = true;
        await db.SaveChangesAsync();
        return dbItem;
    }

    public async Task<Item> SetBackdrop(Guid itemId, Guid imageId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        var img = dbItem.GetImages().FirstOrDefault(x => x.Id == imageId) ?? throw new KeyNotFoundException();
        dbItem.BackdropBase64 = await tmDb.GetImageBase64(img.UrlPath, "w300");
        dbItem.BackdropUrl = img.UrlPath;
        dbItem.Expanded = true;
        await db.SaveChangesAsync();
        return dbItem;
    }

    public async Task<Item> RefreshItem(Guid itemId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
        var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
        dbItem.CopyFromTmDb(tmDbItem, images);
        await db.SaveChangesAsync();
        return dbItem;
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

    public async Task<List<Item>> Search(string keyword)
    {
        var tmDbResults = await tmDb.Search(keyword);
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
            BackdropUrl = x.BackdropPath ?? "/img/ph.png"
        }).ToList();
        return items;
    }
}