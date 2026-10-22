using ArenaQuiz.App.ViewModels;

namespace ArenaQuiz.App.Views;

public partial class DifficultyPage : ContentPage
{
    public DifficultyPage(DifficultyViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}