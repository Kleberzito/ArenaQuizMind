using ArenaQuiz.App.Models;

namespace ArenaQuiz.App.Data;

public class PlayerProfileRepository : IPlayerProfileRepository
{
    private readonly DatabaseService _db;

    public PlayerProfileRepository(DatabaseService db) => _db = db;

    public async Task<PlayerProfile> GetProfileAsync()
    {
        var conn = await _db.GetConnectionAsync();
        var profile = await conn.Table<PlayerProfile>().FirstOrDefaultAsync();
        return profile ?? new PlayerProfile { Id = 1, TotalCoins = 0 };
    }

    public async Task<int> AddCoinsAsync(int amount)
    {
        var conn = await _db.GetConnectionAsync();
        var profile = await GetProfileAsync();
        profile.TotalCoins += amount;
        await conn.UpdateAsync(profile);
        return profile.TotalCoins;
    }

    public async Task UpdateLastPlayedAsync()
    {
        var conn = await _db.GetConnectionAsync();
        var profile = await GetProfileAsync();
        profile.LastPlayedAt = DateTime.UtcNow;
        await conn.UpdateAsync(profile);
    }
}
