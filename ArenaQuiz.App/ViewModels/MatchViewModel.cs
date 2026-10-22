using ArenaQuiz.App.Services;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Dispatching;

namespace ArenaQuiz.App.ViewModels;

public class MatchViewModel : ObservableObject
{
    private static readonly Color CorrectColor = Color.FromArgb("#00E676");
    private static readonly Color WrongColor = Color.FromArgb("#FF3B30");
    private static readonly Color DefaultOptionColor = Color.FromArgb("#1E293B");

    private readonly IMatchService _matchService;
    private IDispatcherTimer? _timer;
    private int _currentTimeLeft;
    private int _totalTimeForQuestion;
    private bool _isProcessing;

    private string _questionText = string.Empty;
    public string QuestionText { get => _questionText; set => SetProperty(ref _questionText, value); }

    private string _optionAText = string.Empty;
    public string OptionAText { get => _optionAText; set => SetProperty(ref _optionAText, value); }
    private string _optionBText = string.Empty;
    public string OptionBText { get => _optionBText; set => SetProperty(ref _optionBText, value); }
    private string _optionCText = string.Empty;
    public string OptionCText { get => _optionCText; set => SetProperty(ref _optionCText, value); }
    private string _optionDText = string.Empty;
    public string OptionDText { get => _optionDText; set => SetProperty(ref _optionDText, value); }

    private Color _optionABg = DefaultOptionColor;
    public Color OptionABg { get => _optionABg; set => SetProperty(ref _optionABg, value); }
    private Color _optionBBg = DefaultOptionColor;
    public Color OptionBBg { get => _optionBBg; set => SetProperty(ref _optionBBg, value); }
    private Color _optionCBg = DefaultOptionColor;
    public Color OptionCBg { get => _optionCBg; set => SetProperty(ref _optionCBg, value); }
    private Color _optionDBg = DefaultOptionColor;
    public Color OptionDBg { get => _optionDBg; set => SetProperty(ref _optionDBg, value); }

    private int _coinsInMatch;
    public int CoinsInMatch { get => _coinsInMatch; set => SetProperty(ref _coinsInMatch, value); }
    private int _wrongAnswers;
    public int WrongAnswers { get => _wrongAnswers; set => SetProperty(ref _wrongAnswers, value); }
    private string _progressText = "1 / 10";
    public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }
    private double _progressBarValue;
    public double ProgressBarValue { get => _progressBarValue; set => SetProperty(ref _progressBarValue, value); }
    private string _timerDisplay = string.Empty;
    public string TimerDisplay { get => _timerDisplay; set => SetProperty(ref _timerDisplay, value); }
    private double _timerProgress;
    public double TimerProgress { get => _timerProgress; set => SetProperty(ref _timerProgress, value); }
    private bool _hasTimer;
    public bool HasTimer { get => _hasTimer; set => SetProperty(ref _hasTimer, value); }

    public ICommand SelectOptionCommand { get; }
    public ICommand GiveUpCommand { get; }

    public MatchViewModel(IMatchService matchService)
    {
        _matchService = matchService;
        SelectOptionCommand = new Command<string>(async o => await SelectOptionAsync(o));
        GiveUpCommand = new Command(async () => await GiveUpAsync());
    }

    public async Task InitializeAsync()
    {
        var match = _matchService.CurrentMatch
            ?? throw new InvalidOperationException("Nenhuma partida ativa.");

        _totalTimeForQuestion = _matchService.GetTimeLimitSeconds(match.Difficulty);
        HasTimer = _totalTimeForQuestion > 0;

        if (HasTimer)
        {
            var dispatcher = Microsoft.Maui.Controls.Application.Current!.Dispatcher;
            _timer = dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        LoadCurrentQuestion();
        await Task.CompletedTask;
    }

    private void LoadCurrentQuestion()
    {
        var match = _matchService.CurrentMatch!;
        var q = match.CurrentQuestion!;

        QuestionText = q.Text;
        OptionAText = q.OptionA;
        OptionBText = q.OptionB;
        OptionCText = q.OptionC;
        OptionDText = q.OptionD;

        ResetOptionColors();
        CoinsInMatch = match.CoinsInMatch;
        WrongAnswers = match.WrongAnswers;
        ProgressText = $"{match.CurrentIndex + 1} / {match.TotalQuestions}";
        ProgressBarValue = (double)match.CurrentIndex / match.TotalQuestions;

        StartTimerIfNeeded();
    }

    private void StartTimerIfNeeded()
    {
        if (!HasTimer || _timer is null) return;
        _currentTimeLeft = _totalTimeForQuestion;
        UpdateTimerUI();
        _timer.Start();
    }

    private void StopTimer() => _timer?.Stop();

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _currentTimeLeft--;
        UpdateTimerUI();
        if (_currentTimeLeft <= 0)
        {
            StopTimer();
            _ = HandleTimeoutAsync();
        }
    }

    private void UpdateTimerUI()
    {
        TimerDisplay = $"{_currentTimeLeft}s";
        TimerProgress = _totalTimeForQuestion > 0
            ? (double)_currentTimeLeft / _totalTimeForQuestion
            : 0;
    }

    private async Task SelectOptionAsync(string? optionIndex)
    {
        if (_isProcessing) return;
        if (!int.TryParse(optionIndex, out var idx)) return;
        await ProcessAnswerAsync(idx);
    }

    private async Task HandleTimeoutAsync()
    {
        if (_isProcessing) return;
        await ProcessAnswerAsync(-1);
    }

    private async Task ProcessAnswerAsync(int selectedIndex)
    {
        if (_isProcessing) return;
        _isProcessing = true;
        StopTimer();

        try
        {
            var result = selectedIndex >= 0
                ? await _matchService.SubmitAnswerAsync(selectedIndex)
                : await _matchService.HandleTimeoutAsync();

            HighlightCorrect(result.CorrectIndex);
            if (selectedIndex >= 0 && !result.IsCorrect)
                HighlightWrong(selectedIndex);

            CoinsInMatch = result.CoinsInMatch;
            WrongAnswers = result.WrongAnswers;

            await Task.Delay(1500);

            if (result.IsGameOver)
                await Shell.Current.GoToAsync($"result?isVictory={result.IsVictory}");
            else
                LoadCurrentQuestion();
        }
        finally { _isProcessing = false; }
    }

    private void ResetOptionColors()
    {
        OptionABg = DefaultOptionColor;
        OptionBBg = DefaultOptionColor;
        OptionCBg = DefaultOptionColor;
        OptionDBg = DefaultOptionColor;
    }

    private void HighlightCorrect(int index) => SetOptionColor(index, CorrectColor);
    private void HighlightWrong(int index) => SetOptionColor(index, WrongColor);

    private void SetOptionColor(int index, Color color)
    {
        switch (index)
        {
            case 0: OptionABg = color; break;
            case 1: OptionBBg = color; break;
            case 2: OptionCBg = color; break;
            case 3: OptionDBg = color; break;
        }
    }    

    private async Task GiveUpAsync()
    {
        if (_isProcessing) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Desistir",
            "Deseja encerrar a partida? Você perderá as moedas ganhas nesta partida.",
            "Sim", "Não");
        if (!confirm) return;

        _isProcessing = true;
        StopTimer();

        try
        {
            await _matchService.AbandonMatchAsync();
            await Shell.Current.GoToAsync("result?isVictory=false");
        }
        finally { _isProcessing = false; }
    }
}