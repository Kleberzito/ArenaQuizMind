namespace ArenaQuiz.App.Models;

public sealed class MatchState
{
    public GameMode Mode { get; init; }
    public string? Theme { get; init; }
    public DifficultyLevel Difficulty { get; init; }
    public List<Question> Questions { get; init; } = new();

    public int CurrentIndex { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int CoinsInMatch { get; set; }

    public MatchPhase Phase { get; set; } = MatchPhase.NotStarted;
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }

    public Question? CurrentQuestion =>
        CurrentIndex >= 0 && CurrentIndex < Questions.Count ? Questions[CurrentIndex] : null;

    public int TotalQuestions => Questions.Count;
    public bool HasNextQuestion => CurrentIndex + 1 < Questions.Count;
}