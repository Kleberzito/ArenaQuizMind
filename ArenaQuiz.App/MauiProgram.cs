using ArenaQuiz.App.Data;
using ArenaQuiz.App.Services;
using ArenaQuiz.App.ViewModels;
using ArenaQuiz.App.Views;
using Microsoft.Extensions.Logging;

namespace ArenaQuiz.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Dados ──────────────────────────────────────────────
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<IQuestionRepository, QuestionRepository>();
        builder.Services.AddSingleton<IPlayerProfileRepository, PlayerProfileRepository>();
        builder.Services.AddSingleton<IMatchHistoryRepository, MatchHistoryRepository>();

        // ── Serviços ───────────────────────────────────────────
        builder.Services.AddSingleton<IMatchService, MatchService>();

        // ── ViewModels ─────────────────────────────────────────
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ThemeViewModel>();
        builder.Services.AddTransient<DifficultyViewModel>();
        builder.Services.AddTransient<MatchViewModel>();
        builder.Services.AddTransient<ResultViewModel>();

        // ── Pages ──────────────────────────────────────────────
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ThemePage>();
        builder.Services.AddTransient<DifficultyPage>();
        builder.Services.AddTransient<MatchPage>();
        builder.Services.AddTransient<ResultPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}