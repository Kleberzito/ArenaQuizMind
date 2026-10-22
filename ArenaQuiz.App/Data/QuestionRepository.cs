using ArenaQuiz.App.Models;

namespace ArenaQuiz.App.Data;

public class QuestionRepository : IQuestionRepository
{
    private readonly DatabaseService _db;

    public QuestionRepository(DatabaseService db) => _db = db;

    public async Task<List<Question>> GetByThemeAsync(string theme)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<Question>()
                         .Where(q => q.Theme == theme)
                         .ToListAsync();
    }

    public async Task<int> GetCountByThemeAsync(string theme)
    {
        var conn = await _db.GetConnectionAsync();
        return await conn.Table<Question>()
                         .Where(q => q.Theme == theme)
                         .CountAsync();
    }

    public async Task<List<string>> GetAllThemesAsync()
    {
        var conn = await _db.GetConnectionAsync();
        var all = await conn.Table<Question>().ToListAsync();
        return all.Select(q => q.Theme).Distinct().OrderBy(t => t).ToList();
    }
}