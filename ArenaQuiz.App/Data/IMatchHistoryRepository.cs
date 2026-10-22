using ArenaQuiz.App.Models;

namespace ArenaQuiz.App.Data;

public interface IMatchHistoryRepository
{
    Task<int> SaveAsync(MatchHistory match);
    Task<List<MatchHistory>> GetRecentAsync(int count = 20);
    Task<List<MatchHistory>> GetAllAsync();
}