using Android.Media;
using ArenaQuiz.App.Views;

namespace ArenaQuiz.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("theme", typeof(ThemePage));
        Routing.RegisterRoute("difficulty", typeof(DifficultyPage));
        Routing.RegisterRoute("match", typeof(MatchPage));
        Routing.RegisterRoute("result", typeof(ResultPage));
    }
}