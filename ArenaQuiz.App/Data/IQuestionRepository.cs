using ArenaQuiz.App.Models;

namespace ArenaQuiz.App.Data;

public interface IQuestionRepository
{
    Task<List<Question>> GetByThemeAsync(string theme);
    Task<int> GetCountByThemeAsync(string theme);
    Task<List<string>> GetAllThemesAsync();
}