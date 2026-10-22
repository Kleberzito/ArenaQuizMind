using ArenaQuiz.App.Models;
using SQLite;
using System.Text.Json;

namespace ArenaQuiz.App.Data;

public class DatabaseService
{
    private const string DbFileName = "arenaquiz.db3";
    private const string SeedFileName = "questions.seed.json";

    private SQLiteAsyncConnection? _connection;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null) return _connection;

        await _initLock.WaitAsync();
        try
        {
            if (_connection is not null) return _connection;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbFileName);
            var conn = new SQLiteAsyncConnection(dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            // Criar tabelas
            await conn.CreateTableAsync<Question>();
            await conn.CreateTableAsync<PlayerProfile>();
            await conn.CreateTableAsync<MatchHistory>();

            // Seed de perguntas (apenas na primeira execução)
            var count = await conn.Table<Question>().CountAsync();
            if (count == 0)
            {
                await SeedQuestionsAsync(conn);
            }

            // Perfil inicial do jogador
            var profile = await conn.Table<PlayerProfile>().FirstOrDefaultAsync();
            if (profile is null)
            {
                await conn.InsertAsync(new PlayerProfile { Id = 1, TotalCoins = 0 });
            }

            _connection = conn;
            return conn;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task SeedQuestionsAsync(SQLiteAsyncConnection conn)
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(SeedFileName);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var seedQuestions = JsonSerializer.Deserialize<List<SeedQuestion>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (seedQuestions is null || seedQuestions.Count == 0) return;

            var questions = seedQuestions.Select(s => new Question
            {
                Text = s.Text,
                OptionA = s.OptionA,
                OptionB = s.OptionB,
                OptionC = s.OptionC,
                OptionD = s.OptionD,
                CorrectIndex = s.CorrectIndex,
                Theme = s.Theme
            }).ToList();

            await conn.InsertAllAsync(questions);
        }
        catch (Exception ex)
        {
            // Em desenvolvimento, falha de seed não derruba o app
            System.Diagnostics.Debug.WriteLine($"[Seed] Falha ao carregar seed: {ex.Message}");
            throw;
        }
    }

    /// <summary>Representação do JSON de seed (sem Id, que é gerado pelo banco).</summary>
    private class SeedQuestion
    {
        public string Text { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public int CorrectIndex { get; set; }
        public string Theme { get; set; } = "Geral";
    }
}