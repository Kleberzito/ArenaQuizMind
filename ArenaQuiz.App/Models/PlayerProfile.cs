using SQLite;

namespace ArenaQuiz.App.Models;

[Table("PlayerProfile")]
public class PlayerProfile
{
    [PrimaryKey]
    public int Id { get; set; } = 1;   // Perfil único no MVP

    public int TotalCoins { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastPlayedAt { get; set; }
}