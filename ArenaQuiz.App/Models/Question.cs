using SQLite;

namespace ArenaQuiz.App.Models;

[Table("Questions")]
public class Question
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Text { get; set; } = string.Empty;

    [NotNull]
    public string OptionA { get; set; } = string.Empty;

    [NotNull]
    public string OptionB { get; set; } = string.Empty;

    [NotNull]
    public string OptionC { get; set; } = string.Empty;

    [NotNull]
    public string OptionD { get; set; } = string.Empty;

    /// <summary>Índice da alternativa correta (0=A, 1=B, 2=C, 3=D).</summary>
    public int CorrectIndex { get; set; }

    /// <summary>Tema da pergunta. "Geral" para o modo Clássico.</summary>
    [NotNull, Indexed]
    public string Theme { get; set; } = "Geral";

    public string[] GetOptions() => new[] { OptionA, OptionB, OptionC, OptionD };
}