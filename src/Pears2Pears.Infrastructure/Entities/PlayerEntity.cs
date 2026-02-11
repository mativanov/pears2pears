using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Infrastructure.Entities;

/// Entity Framework entity for persisting players to the database.
public class PlayerEntity
{
    /// Primary key.
    public Guid Id { get; set; }

    /// Player's display name.
    public string Nickname { get; set; } = string.Empty;

    /// Player's current score (stored as int).
    public int Score { get; set; }

    /// Player's current role.
    public PlayerRole Role { get; set; }

    /// Foreign key to the game.
    public Guid GameId { get; set; }

    /// Is player currently connected.
    public bool IsConnected { get; set; }

    /// When the player joined.
    public DateTime JoinedAt { get; set; }

    /// Last activity timestamp.
    public DateTime LastActivityAt { get; set; }

    /// Is this player the host.
    public bool IsHost { get; set; }

    // Navigation properties

    /// The game this player belongs to.
    public GameEntity Game { get; set; } = null!;

    /// Many-to-many relationship: Cards in this player's hand.
    public ICollection<PlayerCardEntity> Hand { get; set; } = new List<PlayerCardEntity>();

    /// Rounds where this player was the judge.
    public ICollection<RoundEntity> RoundsAsJudge { get; set; } = new List<RoundEntity>();

    /// Rounds where this player was the winner.
    public ICollection<RoundEntity> RoundsAsWinner { get; set; } = new List<RoundEntity>();
}