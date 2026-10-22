using ArenaQuiz.App.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace ArenaQuiz.App.ViewModels;

public class ThemeViewModel : ObservableObject
{
    private readonly IQuestionRepository _questionRepo;

    private List<string> _themes = new();
    public List<string> Themes { get => _themes; set => SetProperty(ref _themes, value); }

    private bool _isBusy;
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public ICommand SelectThemeCommand { get; }

    public ThemeViewModel(IQuestionRepository questionRepo)
    {
        _questionRepo = questionRepo;
        SelectThemeCommand = new Command<string>(async theme => await SelectThemeAsync(theme));
    }

    public async Task LoadAsync()
    {
        var all = await _questionRepo.GetAllThemesAsync();
        Themes = all.Where(t => t != "Geral").ToList();
    }

    private async Task SelectThemeAsync(string? theme)
    {
        if (IsBusy || string.IsNullOrWhiteSpace(theme)) return;
        IsBusy = true;
        try
        {
            await Shell.Current.GoToAsync($"difficulty?mode=Specific&theme={Uri.EscapeDataString(theme)}");
        }
        finally { IsBusy = false; }
    }
}