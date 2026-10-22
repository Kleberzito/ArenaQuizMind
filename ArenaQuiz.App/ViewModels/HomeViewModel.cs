using ArenaQuiz.App.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace ArenaQuiz.App.ViewModels;

public class HomeViewModel : ObservableObject
{
    private readonly IPlayerProfileRepository _profileRepo;

    private int _totalCoins;
    public int TotalCoins { get => _totalCoins; set => SetProperty(ref _totalCoins, value); }

    private bool _isBusy;
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public ICommand PlayClassicCommand { get; }
    public ICommand PlayThemesCommand { get; }

    public ICommand ExitCommand { get; }

    public HomeViewModel(IPlayerProfileRepository profileRepo)
    {
        _profileRepo = profileRepo;
        PlayClassicCommand = new Command(async () => await PlayClassicAsync());
        PlayThemesCommand = new Command(async () => await PlayThemesAsync());
        ExitCommand = new Command(async () => await ExitAppAsync());
    }

    private async Task ExitAppAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Sair", "Deseja fechar o Arena Quiz?", "Sim", "Não");
        if (!confirm) return;
    #if ANDROID
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
    #endif
    }

    public async Task RefreshAsync()
    {
        var profile = await _profileRepo.GetProfileAsync();
        TotalCoins = profile.TotalCoins;
    }

    private async Task PlayClassicAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try { await Shell.Current.GoToAsync("difficulty?mode=Classic"); }
        finally { IsBusy = false; }
    }

    private async Task PlayThemesAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try { await Shell.Current.GoToAsync("theme"); }
        finally { IsBusy = false; }
    }
}