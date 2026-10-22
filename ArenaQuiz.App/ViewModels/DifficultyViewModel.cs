using ArenaQuiz.App.Models;
using ArenaQuiz.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using GameMode = ArenaQuiz.App.Models.GameMode;

namespace ArenaQuiz.App.ViewModels;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(SelectedTheme), "theme")]
public class DifficultyViewModel : ObservableObject
{
    private readonly IMatchService _matchService;

    private string _mode = "Classic";
    public string Mode { get => _mode; set => SetProperty(ref _mode, value); }

    private string? _selectedTheme;
    public string? SelectedTheme { get => _selectedTheme; set => SetProperty(ref _selectedTheme, value); }

    private bool _isBusy;
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public string EasyInfo => "10 perguntas · sem tempo";
    public string MediumInfo => "30 perguntas · 60s por pergunta";
    public string HardInfo => "50 perguntas · 30s por pergunta";

    public ICommand StartCommand { get; }

    public DifficultyViewModel(IMatchService matchService)
    {
        _matchService = matchService;
        StartCommand = new Command<string>(async d => await StartAsync(d));
    }

    private async Task StartAsync(string? difficultyName)
    {
        if (IsBusy || string.IsNullOrWhiteSpace(difficultyName)) return;
        IsBusy = true;
        try
        {
            if (!Enum.TryParse<DifficultyLevel>(difficultyName, out var difficulty)) return;
            var gameMode = Enum.TryParse<GameMode>(Mode, out var m) ? m : GameMode.Classic;

            await _matchService.StartMatchAsync(gameMode, difficulty, SelectedTheme);
            await Shell.Current.GoToAsync("match");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }
}