using ArenaQuiz.App.ViewModels;

namespace ArenaQuiz.App.Views;

public partial class ResultPage : ContentPage
{
    private readonly ResultViewModel _vm;

    public ResultPage(ResultViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Load();
    }
}