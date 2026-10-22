using ArenaQuiz.App.ViewModels;

namespace ArenaQuiz.App.Views;

public partial class MatchPage : ContentPage
{
    private readonly MatchViewModel _vm;

    public MatchPage(MatchViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}