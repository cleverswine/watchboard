using Microsoft.EntityFrameworkCore;
using WatchBoard.Database;
using WatchBoard.Database.Entities;
using WatchBoard.Services.TmDb;
using WatchBoard.Services.TmDb.Models;

namespace WatchBoard.Services;

public interface IRepository
{
    Task<List<Board>> GetBoards();
    Task<Board?> GetBoard(Guid? boardId);
    Task<Board> CreateBoard(string name);
    Task ReorderBoards(Guid[] boardIds);
    Task RenameBoard(Guid boardId, string name);
    Task DeleteBoard(Guid boardId);

    Task<List?> GetList(Guid listId);
    Task<List> CreateList(Guid boardId, string name);
    Task ReorderLists(Guid boardId, Guid[] listIds);
    Task RenameList(Guid listId, string name);
    Task SetDefaultList(Guid listId);
    Task DeleteList(Guid listId);
    Task<int> GetListItemCount(Guid listId);
    Task SortList(Guid listId, string?[] itemIdsStr);

    Task<Item[]> GetItems();
    Task<Item?> GetItem(Guid itemId);
    Task<List<Item>> GetAllItemsWithDetails();
    Task<(Item Item, string BoardName, string ListName)?> GetItemWithDetails(Guid itemId);
    Task<Item> AddItemToBoard(Guid? boardId, int tmDbId, string type);
    Task MoveItemToOtherBoard(Guid itemId, Guid boardId);
    Task MoveItemToOtherList(Guid itemId, Guid listId);
    Task<Item> SetItemProvider(Guid itemId, int? providerId);
    Task<Item> RefreshItem(Guid itemId);
    Task DeleteItem(Guid itemId);
    Task<List<Item>> SearchForItems(string keyword, ItemType itemType);

    Task AddSystemLog(SystemLog log);
    Task<(List<SystemLog> Logs, int TotalCount)> GetSystemLogs(int page, int pageSize);
    Task<int> CleanSystemLogs(DateTimeOffset cutOffDateTime);
}

public class Repository(AppDbContext db, ITmDb tmDb) : IRepository
{
    public async Task<Board?> GetBoard(Guid? boardId)
    {
        var boards = db.Boards
            .AsNoTracking()
            .Include(x => x.Lists.OrderByDescending(l => l.Order))
            .ThenInclude(x => x.Items.OrderBy(i => i.Order));
        if (boardId.HasValue)
            return await boards.FirstOrDefaultAsync(x => x.Id == boardId);
        return await boards.FirstOrDefaultAsync();
    }

    public async Task<List<Board>> GetBoards()
    {
        return await db.Boards
            .OrderBy(x => x.Order)
            .Include(x => x.Lists.OrderBy(l => l.Order))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Board> CreateBoard(string name)
    {
        var maxOrder = await db.Boards.MaxAsync(b => (int?)b.Order) ?? -1;

        var board = new Board
        {
            Name = name,
            Order = maxOrder + 1,
            Lists =
            [
                new List { Name = "Queue", Order = 0, Default = true, Items = [] },
                new List { Name = "Watching", Order = 1, Items = [] },
                new List { Name = "Finished", Order = 2, Items = [] }
            ]
        };

        db.Boards.Add(board);
        await db.SaveChangesAsync();

        return board;
    }

    public async Task ReorderBoards(Guid[] boardIds)
    {
        for (var i = 0; i < boardIds.Length; i++)
        {
            var board = await db.Boards.FindAsync(boardIds[i]);
            if (board != null)
            {
                board.Order = i;
            }
        }
        await db.SaveChangesAsync();
    }

    public async Task RenameBoard(Guid boardId, string name)
    {
        var board = await db.Boards.FindAsync(boardId);
        if (board != null)
        {
            board.Name = name;
            await db.SaveChangesAsync();
        }
    }

    public async Task DeleteBoard(Guid boardId)
    {
        var board = await db.Boards
            .Include(b => b.Lists)
            .ThenInclude(l => l.Items)
            .FirstOrDefaultAsync(b => b.Id == boardId);

        if (board != null)
        {
            // Remove all items from lists
            foreach (var list in board.Lists)
            {
                db.Items.RemoveRange(list.Items);
            }

            // Remove all lists
            db.Lists.RemoveRange(board.Lists);

            // Remove the board
            db.Boards.Remove(board);

            await db.SaveChangesAsync();
        }
    }

    public async Task<List?> GetList(Guid listId)
    {
        return await db.Lists
            .AsNoTracking()
            .Include(x => x.Items.OrderBy(y => y.Order))
            .FirstOrDefaultAsync(x => x.Id == listId);
    }

    public async Task<List> CreateList(Guid boardId, string name)
    {
        var maxOrder = await db.Lists
            .Where(l => l.BoardId == boardId)
            .MaxAsync(l => (int?)l.Order) ?? -1;

        var list = new List
        {
            Name = name,
            BoardId = boardId,
            Order = maxOrder + 1,
            Default = false,
            Items = []
        };

        db.Lists.Add(list);
        await db.SaveChangesAsync();

        return list;
    }

    public async Task ReorderLists(Guid boardId, Guid[] listIds)
    {
        for (var i = 0; i < listIds.Length; i++)
        {
            var list = await db.Lists.FindAsync(listIds[i]);
            if (list != null && list.BoardId == boardId)
            {
                list.Order = i;
            }
        }
        await db.SaveChangesAsync();
    }

    public async Task RenameList(Guid listId, string name)
    {
        var list = await db.Lists.FindAsync(listId);
        if (list != null)
        {
            list.Name = name;
            await db.SaveChangesAsync();
        }
    }

    public async Task SetDefaultList(Guid listId)
    {
        var list = await db.Lists.FindAsync(listId);
        if (list == null) return;

        // Clear default from all lists in the same board
        var boardLists = await db.Lists.Where(l => l.BoardId == list.BoardId).ToListAsync();
        foreach (var l in boardLists)
        {
            l.Default = l.Id == listId;
        }
        await db.SaveChangesAsync();
    }

    public async Task DeleteList(Guid listId)
    {
        var list = await db.Lists
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == listId);

        if (list != null)
        {
            // Remove all items from the list
            db.Items.RemoveRange(list.Items);

            // Remove the list
            db.Lists.Remove(list);

            await db.SaveChangesAsync();
        }
    }

    public async Task<int> GetListItemCount(Guid listId)
    {
        return await db.Items
            .AsNoTracking()
            .CountAsync(x => x.ListId == listId);
    }

    public async Task<Item[]> GetItems()
    {
        return await db.Items.AsNoTracking().ToArrayAsync();
    }

    public async Task<Item?> GetItem(Guid itemId)
    {
        return await db.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == itemId);
    }

    public async Task<List<Item>> GetAllItemsWithDetails()
    {
        return await db.Items
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<(Item Item, string BoardName, string ListName)?> GetItemWithDetails(Guid itemId)
    {
        var item = await db.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == itemId);

        if (item == null) return null;

        var list = await db.Lists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == item.ListId);

        if (list == null) return (item, "Unknown", "Unknown");

        var board = await db.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == list.BoardId);

        return (item, board?.Name ?? "Unknown", list.Name);
    }

    public async Task<Item> AddItemToBoard(Guid? boardId, int tmDbId, string type)
    {
        var listId = db.Lists.AsNoTracking().FirstOrDefault(x => x.Default == true && x.BoardId == boardId)?.Id
                     ?? db.Lists.AsNoTracking().FirstOrDefault(x => x.BoardId == boardId)?.Id
                     ?? throw new KeyNotFoundException();

        var newListItems = db.Items.AsNoTracking()
            .Where(x => x.ListId == listId).ToList();
        var maxOrder = newListItems.Count > 0 ? newListItems.Max(x => x.Order) : 0;

        var dbItem = new Item
        {
            Type = type == "tv"
                ? ItemType.Tv
                : ItemType.Movie,
            TmdbId = tmDbId,
            ListId = listId,
            Order = maxOrder + 1
        };
        await UpdateItemFromTmDb(dbItem);
        db.Items.Add(dbItem);
        await db.SaveChangesAsync();
        return dbItem;
    }

    public async Task<Item> RefreshItem(Guid itemId)
    {
        var dbItem = await db.Items.FindAsync(itemId) ?? throw new KeyNotFoundException();
        var previousItemHash = dbItem.ItemHash;

        await UpdateItemFromTmDb(dbItem);
        var newItemHash = $"0x{System.Text.Json.JsonSerializer.Serialize(dbItem).GetHashCode():X8}";
        dbItem.ItemHash = newItemHash;
        
        if (previousItemHash != newItemHash)
        {
            await db.SaveChangesAsync();
            await AddSystemLog(new SystemLog
            {
                Type = SystemLogType.ItemRefreshed, ItemId = dbItem.Id,
                Message = $"Item \"{dbItem.Name}\" was updated from TMDB."
            });
        }
        else
        {
            db.Entry(dbItem).State = EntityState.Unchanged;
        }

        return dbItem;
    }

    public async Task MoveItemToOtherBoard(Guid itemId, Guid boardId)
    {
        var item = await db.Items.FindAsync(itemId);
        if (item == null) return;

        var newListId = db.Lists.AsNoTracking().FirstOrDefault(x => x.Default == true && x.BoardId == boardId)?.Id
                        ?? db.Lists.AsNoTracking().FirstOrDefault(x => x.BoardId == boardId)?.Id
                        ?? throw new KeyNotFoundException();

        var newListItems = db.Items.AsNoTracking()
            .Where(x => x.ListId == newListId).ToList();
        var maxOrder = newListItems.Count > 0 ? newListItems.Max(x => x.Order) : 0;

        item.ListId = newListId;
        item.Order = maxOrder + 1;
        await db.SaveChangesAsync();
    }

    public async Task MoveItemToOtherList(Guid itemId, Guid listId)
    {
        var item = await db.Items.FindAsync(itemId);
        if (item == null) return;

        var newListItems = db.Items.AsNoTracking()
            .Where(x => x.ListId == listId).ToList();
        var maxOrder = newListItems.Count > 0 ? newListItems.Max(x => x.Order) : 0;

        item.ListId = listId;
        item.Order = maxOrder + 1;
        await db.SaveChangesAsync();
    }

    public async Task<Item> SetItemProvider(Guid itemId, int? providerId)
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

    public async Task DeleteItem(Guid itemId)
    {
        await db.SystemLogs.Where(x => x.ItemId == itemId).ExecuteDeleteAsync();
        
        var item = await db.Items.FindAsync(itemId);
        if (item == null) return;
        db.Remove(item);
        await db.SaveChangesAsync();
    }

    public async Task SortList(Guid listId, string?[] itemIdsStr)
    {
        var dbItems = db.Items.Where(x => x.ListId == listId).ToList();

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
                    dbItem.ListId = listId;
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
        List<TmDbItem> tmDbResults;

        if (keyword.Contains("www.imdb.com"))
        {
            var xs = keyword.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (xs.Length < 4)
                tmDbResults = await tmDb.Search(keyword, itemType.ToString().ToLower());
            else
                tmDbResults = [await tmDb.GetDetailByImDbId(xs[3])];
        }
        else
        {
            tmDbResults = await tmDb.Search(keyword, itemType.ToString().ToLower());
        }

        var items = tmDbResults.Select(x => new Item
        {
            Id = Guid.Empty,
            TmdbId = x.Id,
            Name = x.ItemName ?? "UNKNOWN",
            Type = x.MediaType == "tv" ? ItemType.Tv : ItemType.Movie,
            TagLine = x.TagLine,
            ReleaseDate = x.ItemReleaseDate,
            EndDate = x.LastAirDate,
            NumberOfSeasons = x.NumberOfSeasons,
            PosterUrl = x.PosterPath ?? "",
            OriginalLanguage = x.OriginalLanguage?.ToUpper() ?? "",
            OriginCountry = string.Join(", ", x.OriginCountry),
            Overview = x.Overview
        }).ToList();
        return items;
    }

    public async Task AddSystemLog(SystemLog log)
    {
        db.SystemLogs.Add(log);
        await db.SaveChangesAsync();
    }

    public async Task<(List<SystemLog> Logs, int TotalCount)> GetSystemLogs(int page, int pageSize)
    {
        var totalCount = await db.SystemLogs.CountAsync();
        var logs = await db.SystemLogs
            .AsNoTracking()
            .Include(x => x.Item)
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (logs, totalCount);
    }

    public async Task<int> CleanSystemLogs(DateTimeOffset cutOffDateTime)
    {
        return await db.SystemLogs.Where(x => x.Timestamp < cutOffDateTime).ExecuteDeleteAsync();
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

        dbItem.PosterBase64 = await tmDb.GetImageBase64(dbItem.PosterUrl, "w185");
        dbItem.BackdropBase64 = await tmDb.GetImageBase64(dbItem.BackdropUrl, "w780");
    }
}