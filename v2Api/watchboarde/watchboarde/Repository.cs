using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WatchBoard.Database;
using WatchBoard.Database.Entities;
using WatchBoard.TmDb;
using WatchBoard.TmDb.Models;

namespace WatchBoard;

public interface IRepository
{
    Task<Item> AddOrUpdateItem(int tmDbId, ItemType itemType, Guid? boardId = null);
    Task MoveItem(Guid itemId, Guid boardId);
    Task ReorderItems(Guid listId, Guid[] newOrder);
}

public class Repository(ITmDbService tmDbService, AppDbContext db) : IRepository
{
    public async Task MoveItem(Guid itemId, Guid boardId)
    {
        var item = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException("Item not found");
        var listId = db.Lists.FirstOrDefault(x => x.BoardId == boardId && x.Default == true)?.Id ?? throw new Exception("List not found");
        item.BoardListId = listId;
        await db.SaveChangesAsync();
    }

    public async Task ReorderItems(Guid listId, Guid[] newOrder)
    {
        var items = await db.Items.Where(x => x.BoardListId == listId).ToListAsync();
        var i = 0;
        foreach (var id in newOrder)
        {
            var item = items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.Order = i++;
            }
        }

        await db.SaveChangesAsync();
    }

    public async Task<Item> AddOrUpdateItem(int tmDbId, ItemType itemType, Guid? boardId = null)
    {
        var item = await db.Items.FirstOrDefaultAsync(x => x.TmDbId == tmDbId);
        var isNew = item == null;

        if (itemType == ItemType.TvShow)
        {
            var t = await tmDbService.GetTvShow(tmDbId);
            item ??= new Item(t);
            if (isNew)
                item.BoardListId = db.Lists.FirstOrDefault(x => x.BoardId == boardId && x.Default == true)?.Id ??
                                   throw new KeyNotFoundException("List not found");
            await PopulateItemJson(item, t);
            await PopulateItemExtra(item, t);
        }
        else
        {
            var t = await tmDbService.GetMovie(tmDbId);
            item ??= new Item(t);
            if (isNew)
                item.BoardListId = db.Lists.FirstOrDefault(x => x.BoardId == boardId && x.Default == true)?.Id ??
                                   throw new KeyNotFoundException("List not found");
            await PopulateItemJson(item, t);
            await PopulateItemExtra(item, t);
        }

        if (isNew)
        {
            item.BoardListId = db.Lists.FirstOrDefault(x => x.BoardId == boardId && x.Default == true)?.Id ??
                               throw new KeyNotFoundException("List not found");
            await db.Items.AddAsync(item);
        }
        else db.Entry(item).State = EntityState.Modified;

        await db.SaveChangesAsync();

        return await db.Items.FindAsync(item.Id) ?? throw new KeyNotFoundException("Item not found");
    }

    private async Task PopulateItemJson<T>(Item item, T tmDbItem)
    {
        var je = await db.Json.FirstOrDefaultAsync(x => x.ItemId == item.Id);
        if (je == null)
        {
            db.Json.Add(new Json {ItemId = item.Id, Data = JsonSerializer.Serialize(tmDbItem), DataType = tmDbItem!.GetType().Name});
        }
        else
        {
            je.Data = JsonSerializer.Serialize(tmDbItem);
        }
    }

    private async Task PopulateItemExtra(Item item, TmDbItemBase tmDbItem)
    {
        var list = await db.Lists.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == item.BoardListId) ?? throw new KeyNotFoundException("List not found");
        if (!list.Items.Exists(x => x.Id == item.Id))
        {
            item.Order = list.Items.Count > 0 ? list.Items.Max(x => x.Order) + 1 : 0;
        }

        if (tmDbItem.BackdropPath != null)
        {
            var bd = await tmDbService.GetImage(tmDbItem.BackdropPath, "w780");
            var bde = await db.Backdrops.FirstOrDefaultAsync(x => x.ItemId == item.Id);
            if (bde == null)
            {
                await db.Backdrops.AddAsync(new Backdrop {ItemId = item.Id, Data = bd.Data, MediaType = bd.ImageType});
            }
            else
            {
                bde.Data = bd.Data;
                bde.MediaType = bd.ImageType;
            }
        }

        if (tmDbItem.PosterPath != null)
        {
            var p = await tmDbService.GetImage(tmDbItem.PosterPath, "w185");
            var pe = await db.Posters.FirstOrDefaultAsync(x => x.ItemId == item.Id);
            if (pe == null)
            {
                await db.Posters.AddAsync(new Poster {ItemId = item.Id, Data = p.Data, MediaType = p.ImageType});
            }
            else
            {
                pe.Data = p.Data;
                pe.MediaType = p.ImageType;
            }
        }
    }
}