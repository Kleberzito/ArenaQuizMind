using SQLite;

namespace ArenaQuiz.App.Models;

[Table("MatchHistory")]
public class MatchHistory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

    public GameMode Mode { get; set; }

    /// <summary>Tema específico, se Mode = Specific. Null caso Classic.</summary>
    public string? Theme { get; set; }

    public DifficultyLevel Difficulty { get; set; }

    public int TotalQuestions { get; set; }

    public int CorrectAnswers { get; set; }

    public int WrongAnswers { get; set; }

    public bool WasVictory { get; set; }

    /// <summary>Moedas que efetivamente foram para o placar geral (0 se derrota).</summary>
    public int CoinsEarned { get; set; }
}