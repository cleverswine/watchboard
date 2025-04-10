using Microsoft.EntityFrameworkCore;
using WatchBoard.Services.Database;
using WatchBoard.Services.Database.Entities;
using WatchBoard.Services.TmDb;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services;

public interface IRepository
{
    Task<List<Board>> GetBoards();
    Task<Board?> GetBoard(Guid? id);
    Task<Board> UpdateBoard(Guid id, string name);
    Task DeleteBoard(Guid id);
    Task<Board> AddBoard(string name);

    Task<List?> GetList(Guid id);
    Task<List> UpdateList(Guid id, string name);
    Task<List> MoveListUp(Guid id);
    Task<List> MoveListDown(Guid id);
    Task DeleteList(Guid id);
    Task<List> AddList(string name);
    Task SortList(Guid id, string?[] itemIdsStr);

    Task<Item?> GetItem(Guid id);
    Task<Item> AddItemToBoard(Guid? boardId, int tmDbId, string type);
    Task<Item> AddItem(Guid listId, int tmDbId, string type);
    Task MoveItemToOtherBoard(Guid itemId, Guid boardId);
    Task<Item> SetItemProvider(Guid itemId, int providerId);
    Task<Item> SetItemBackdrop(Guid itemId, Guid imageId);
    Task<string> GetItemBackdropUrl(Guid itemId, Guid imageId);
    Task<Item> RefreshItem(Guid itemId);
    Task DeleteItem(Guid id);
    Task<List<Item>> SearchForItems(string keyword);

    Task<List<TmDbProvider>> GetTmDbProviders();
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

    public async Task<Board> UpdateBoard(Guid id, string name)
    {
        var b = await db.Boards.FindAsync(id) ?? throw new KeyNotFoundException();
        b.Name = name;
        await db.SaveChangesAsync();
        return b;
    }

    public async Task DeleteBoard(Guid id)
    {
        var b = await db.Boards
            .Include(x => x.Lists)
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();
        foreach (var l in b.Lists)
        {
            var items = db.Items.Where(x => x.ListId == l.Id);
            db.Items.RemoveRange(items);
            db.Lists.Remove(l);
        }

        db.Boards.Remove(b);
        await db.SaveChangesAsync();
    }

    public async Task<Board> AddBoard(string name)
    {
        throw new NotImplementedException();
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

    public async Task<List> UpdateList(Guid id, string name)
    {
        var list = await db.Lists.FindAsync(id) ?? throw new KeyNotFoundException();
        list.Name = name;
        await db.SaveChangesAsync();
        return list;
    }

    public async Task<List> MoveListUp(Guid id)
    {
        var list = await db.Lists.FindAsync(id) ?? throw new KeyNotFoundException();
        if (list.Order == 0) return list;
        
        var lists = await db.Lists
            .Where(x => x.BoardId == list.BoardId)
            .OrderBy(x => x.Order)
            .ToArrayAsync();
        if (lists.Length == 1) return list;
        
        lists[list.Order - 1].Order = list.Order;
        lists[list.Order].Order = list.Order - 1;
        
        await db.SaveChangesAsync();
        return list;
    }

    public async Task<List> MoveListDown(Guid id)
    {
        var list = await db.Lists.FindAsync(id) ?? throw new KeyNotFoundException();
        
        var lists = await db.Lists
            .Where(x => x.BoardId == list.BoardId)
            .OrderBy(x => x.Order)
            .ToArrayAsync();
        if (list.Order >= lists.Length - 1) return list;
        
        lists[list.Order + 1].Order = list.Order;
        lists[list.Order].Order = list.Order + 1;
        
        await db.SaveChangesAsync();
        return list;
    }

    public async Task DeleteList(Guid id)
    {
        var l = await db.Lists
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();
        foreach (var i in l.Items)
        {
            db.Items.Remove(i);
        }

        db.Lists.Remove(l);
        await db.SaveChangesAsync();
    }

    public async Task<List> AddList(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Item?> GetItem(Guid id)
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
        var dbItem = tmDbItem.MapTmDbToItem(listId, images);
        dbItem.BackdropBase64 = await tmDb.GetImageBase64(dbItem.BackdropUrl);
        dbItem.PosterBase64 = await tmDb.GetImageBase64(dbItem.PosterUrl, "w92");
        dbItem.Order = order;
        db.Items.Add(dbItem);
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

    public async Task<Item> RefreshItem(Guid itemId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        var tmDbItem = await tmDb.GetDetail(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
        var images = await tmDb.GetImages(dbItem.TmdbId, dbItem.Type.ToString().ToLower());
        dbItem.UpdateFromTmDb(tmDbItem, images);
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

    public async Task<List<Item>> SearchForItems(string keyword)
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

    public async Task<List<TmDbProvider>> GetTmDbProviders()
    {
        return await tmDb.GetProviders("tv");
    }
}