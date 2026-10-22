using ArenaQuiz.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace ArenaQuiz.App.ViewModels;

[QueryProperty(nameof(IsVictoryParam), "isVictory")]
public class ResultViewModel : ObservableObject
{
    private readonly IMatchService _matchService;

    private bool _isVictory;
    public bool IsVictory { get => _isVictory; set => SetProperty(ref _isVictory, value); }

    private string _headline = string.Empty;
    public string Headline { get => _headline; set => SetProperty(ref _headline, value); }

    private string _headlineEmoji = string.Empty;
    public string HeadlineEmoji { get => _headlineEmoji; set => SetProperty(ref _headlineEmoji, value); }

    private string _summary = string.Empty;
    public string Summary { get => _summary; set => SetProperty(ref _summary, value); }

    private string _coinsEarnedText = string.Empty;
    public string CoinsEarnedText { get => _coinsEarnedText; set => SetProperty(ref _coinsEarnedText, value); }

    public string? IsVictoryParam
    {
        set => IsVictory = bool.TryParse(value, out var v) && v;
    }

    public ICommand GoHomeCommand { get; }

    public ResultViewModel(IMatchService matchService)
    {
        _matchService = matchService;
        GoHomeCommand = new Command(async () => await GoHomeAsync());
    }

    public void Load()
    {
        var match = _matchService.CurrentMatch;
        if (match is null) return;

        if (match.Phase == Models.MatchPhase.Abandoned)
        {
            IsVictory = false;
            Headline = "DESISTIU";
            HeadlineEmoji = "🚪";
            Summary = $"{match.CorrectAnswers} de {match.TotalQuestions} corretas antes de desistir";
            CoinsEarnedText = "Moedas da partida perdidas";
            return;
        }

        IsVictory = match.Phase == Models.MatchPhase.Victory;
        Headline = IsVictory ? "VITÓRIA!" : "DERROTA";
        HeadlineEmoji = IsVictory ? "🏆" : "💔";
        Summary = $"{match.CorrectAnswers} de {match.TotalQuestions} corretas";

        var coins = IsVictory ? match.CoinsInMatch : 0;
        CoinsEarnedText = IsVictory
            ? $"+{coins} moedas"
            : "0 moedas (moedas da partida perdidas)";
    }

    private async Task GoHomeAsync()
    {
        await Shell.Current.GoToAsync("//home");
    }
}