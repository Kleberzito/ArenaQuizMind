using ArenaQuiz.App.Models;

namespace ArenaQuiz.App.Data;

public interface IPlayerProfileRepository
{
    Task<PlayerProfile> GetProfileAsync();
    Task<int> AddCoinsAsync(int amount);
    Task UpdateLastPlayedAsync();
}