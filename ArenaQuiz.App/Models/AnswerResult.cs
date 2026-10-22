namespace ArenaQuiz.App.Models;

/// <summary>Resultado de uma submissão de resposta (ou timeout).</summary>
public sealed record AnswerResult
{
    public bool IsCorrect { get; init; }
    public int CorrectIndex { get; init; }
    public bool IsGameOver { get; init; }
    public bool IsVictory { get; init; }
    public int CoinsInMatch { get; init; }
    public int CoinsEarned { get; init; }   // só > 0 se vitória
    public int CorrectAnswers { get; init; }
    public int WrongAnswers { get; init; }
    public int TotalQuestions { get; init; }
    public MatchHistory? PersistedHistory { get; init; }
}