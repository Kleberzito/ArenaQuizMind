using ArenaQuiz.App.Models;

namespace ArenaQuiz.App.Data;

public class MatchHistoryRepository : IMatchHistoryRepository
{
    private readonly DatabaseService _db;

    public MatchHistoryRepository(DatabaseService db) => _db = db;

    public async Task<int> SaveAsync(MatchHistory match)
    {
        var conn = await _db.GetConnectionAsync();
        await conn.InsertAsync(match);
        return match.Id;
    }

    public async Task<List<MatchHistory>> GetRecentAsync(int count = 20)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<MatchHistory>()
                         .OrderByDescending(m => m.PlayedAt)
                         .Take(count)
                         .ToListAsync();
    }

    public async Task<List<MatchHistory>> GetAllAsync()
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<MatchHistory>()
                         .OrderByDescending(m => m.PlayedAt)
                         .ToListAsync();
    }
}