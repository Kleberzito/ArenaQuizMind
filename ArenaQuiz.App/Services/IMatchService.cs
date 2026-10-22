using ArenaQuiz.App.Models;
using GameMode = ArenaQuiz.App.Models.GameMode;

namespace ArenaQuiz.App.Services;

public interface IMatchService
{
    MatchState? CurrentMatch { get; }
    Question? CurrentQuestion { get; }

    Task<MatchState> StartMatchAsync(GameMode mode, DifficultyLevel difficulty, string? theme = null);

    Task<AnswerResult> SubmitAnswerAsync(int selectedIndex);
    Task<AnswerResult> HandleTimeoutAsync();

    Task AbandonMatchAsync();

    int GetTotalQuestions(DifficultyLevel difficulty);
    int GetTimeLimitSeconds(DifficultyLevel difficulty);
}