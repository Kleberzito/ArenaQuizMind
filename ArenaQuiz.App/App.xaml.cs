using Android.Views;
using ArenaQuiz.App.Data;
using Window = Microsoft.Maui.Controls.Window;

namespace ArenaQuiz.App;

public partial class App : Application
{
    private readonly DatabaseService _databaseService;

    public App(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _ = InitializeDatabaseAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            await _databaseService.GetConnectionAsync();
            System.Diagnostics.Debug.WriteLine("[Bootstrap] Banco inicializado com sucesso.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Bootstrap] Falha: {ex}");
        }
    }
}