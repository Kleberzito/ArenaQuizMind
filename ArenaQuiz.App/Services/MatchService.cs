using ArenaQuiz.App.Data;
using ArenaQuiz.App.Models;
using GameMode = ArenaQuiz.App.Models.GameMode;

namespace ArenaQuiz.App.Services;

public class MatchService : IMatchService
{
    private const string ClassicTheme = "Geral";

    private readonly IQuestionRepository _questionRepo;
    private readonly IMatchHistoryRepository _historyRepo;
    private readonly IPlayerProfileRepository _profileRepo;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private MatchState? _current;

    public MatchService(
        IQuestionRepository questionRepo,
        IMatchHistoryRepository historyRepo,
        IPlayerProfileRepository profileRepo)
    {
        _questionRepo = questionRepo;
        _historyRepo = historyRepo;
        _profileRepo = profileRepo;
    }

    public MatchState? CurrentMatch => _current;
    public Question? CurrentQuestion => _current?.CurrentQuestion;

    public async Task<MatchState> StartMatchAsync(GameMode mode, DifficultyLevel difficulty, string? theme = null)
    {
        await _lock.WaitAsync();
        try
        {
            // 1. Buscar pool de perguntas
            List<Question> pool = mode switch
            {
                GameMode.Classic => await GetAllQuestionsAsync(),
                GameMode.Specific => !string.IsNullOrWhiteSpace(theme)
                    ? await _questionRepo.GetByThemeAsync(theme)
                    : throw new ArgumentException("Tema é obrigatório no modo Específico.", nameof(theme)),
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };

            if (pool.Count == 0)
                throw new InvalidOperationException("Nenhuma pergunta disponível para esta seleção.");

            // 2. Embaralhar e capar
            var required = GetTotalQuestions(difficulty);
            var selected = pool
                .OrderBy(_ => Random.Shared.Next())
                .Take(required)
                .ToList();

            // 3. Montar o estado
            _current = new MatchState
            {
                Mode = mode,
                Theme = mode == GameMode.Specific ? theme : null,
                Difficulty = difficulty,
                Questions = selected,
                CurrentIndex = 0,
                Phase = MatchPhase.InProgress
            };

            return _current;
        }
        finally
        {
            _lock.Release();
        }
    }

    public Task<AnswerResult> SubmitAnswerAsync(int selectedIndex)
    {
        return ProcessAnswerAsync(selectedIndex);
    }

    public Task<AnswerResult> HandleTimeoutAsync()
    {
        return ProcessAnswerAsync(selectedIndex: -1); // -1 = timeout
    }

    private async Task<AnswerResult> ProcessAnswerAsync(int selectedIndex)
    {
        await _lock.WaitAsync();
        try
        {
            if (_current is null || _current.Phase != MatchPhase.InProgress)
                throw new InvalidOperationException("Nenhuma partida em andamento.");

            var q = _current.CurrentQuestion
                ?? throw new InvalidOperationException("Pergunta atual inválida.");

            var isCorrect = selectedIndex == q.CorrectIndex;

            if (isCorrect)
            {
                _current.CorrectAnswers++;
                _current.CoinsInMatch++;
            }
            else
            {
                _current.WrongAnswers++;
            }

            // Avaliar fim de jogo
            var isDefeat = _current.WrongAnswers >= 3;
            var isLastQuestion = !_current.HasNextQuestion;

            if (isDefeat)
            {
                _current.Phase = MatchPhase.Defeat;
            }
            else if (isLastQuestion)
            {
                _current.Phase = MatchPhase.Victory;
            }
            else
            {
                _current.CurrentIndex++;
            }

            MatchHistory? persisted = null;
            int coinsEarned = 0;

            if (_current.Phase is MatchPhase.Victory or MatchPhase.Defeat)
            {
                persisted = await FinishInternalAsync();
                coinsEarned = _current.Phase == MatchPhase.Victory ? _current.CoinsInMatch : 0;
            }

            return new AnswerResult
            {
                IsCorrect = isCorrect,
                CorrectIndex = q.CorrectIndex,
                IsGameOver = _current.Phase is MatchPhase.Victory or MatchPhase.Defeat,
                IsVictory = _current.Phase == MatchPhase.Victory,
                CoinsInMatch = _current.CoinsInMatch,
                CoinsEarned = coinsEarned,
                CorrectAnswers = _current.CorrectAnswers,
                WrongAnswers = _current.WrongAnswers,
                TotalQuestions = _current.TotalQuestions,
                PersistedHistory = persisted
            };
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AbandonMatchAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (_current is null || _current.Phase != MatchPhase.InProgress) return;
            _current.Phase = MatchPhase.Abandoned;
            await FinishInternalAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>Deve ser chamado dentro do lock. Persiste histórico e atualiza perfil se vitória.</summary>
    private async Task<MatchHistory> FinishInternalAsync()
    {
        var m = _current!;
        m.FinishedAt = DateTime.UtcNow;

        var isVictory = m.Phase == MatchPhase.Victory;
        var coinsEarned = isVictory ? m.CoinsInMatch : 0;

        var history = new MatchHistory
        {
            PlayedAt = m.StartedAt,
            Mode = m.Mode,
            Theme = m.Theme,
            Difficulty = m.Difficulty,
            TotalQuestions = m.TotalQuestions,
            CorrectAnswers = m.CorrectAnswers,
            WrongAnswers = m.WrongAnswers,
            WasVictory = isVictory,
            CoinsEarned = coinsEarned
        };

        await _historyRepo.SaveAsync(history);

        if (isVictory && coinsEarned > 0)
            await _profileRepo.AddCoinsAsync(coinsEarned);

        await _profileRepo.UpdateLastPlayedAsync();

        return history;
    }

    public int GetTotalQuestions(DifficultyLevel difficulty) => difficulty switch
    {
        DifficultyLevel.Easy => 10,
        DifficultyLevel.Medium => 30,
        DifficultyLevel.Hard => 50,
        _ => 10
    };

    public int GetTimeLimitSeconds(DifficultyLevel difficulty) => difficulty switch
    {
        DifficultyLevel.Easy => 0,   // sem timer
        DifficultyLevel.Medium => 60,
        DifficultyLevel.Hard => 30,
        _ => 0
    };

    private async Task<List<Question>> GetAllQuestionsAsync()
    {
        // Para o modo Clássico, usamos todos os temas
        var themes = await _questionRepo.GetAllThemesAsync();
        var all = new List<Question>();
        foreach (var theme in themes)
        {
            var qs = await _questionRepo.GetByThemeAsync(theme);
            all.AddRange(qs);
        }
        return all;
    }
}