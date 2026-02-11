using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Infrastructure.Entities;

/// Entity Framework entity for persisting games to the database.
public class GameEntity
{
    /// Primary key.
    public Guid Id { get; set; }

    /// Unique 6-character game code.
    public string Code { get; set; } = string.Empty;

    /// Current game status.
    public GameStatus Status { get; set; }

    /// Winning score threshold.
    public int WinningScore { get; set; }

    /// Winner player ID (nullable).
    public Guid? WinnerId { get; set; }

    /// Timestamp when game was created.
    public DateTime CreatedAt { get; set; }

    /// Timestamp when game started (nullable).
    public DateTime? StartedAt { get; set; }

    /// Timestamp when game ended (nullable).
    public DateTime? EndedAt { get; set; }

    /// Serialized red card deck (JSON array of Card IDs).
    /// Stores remaining cards in deck.
    public string RedCardDeckJson { get; set; } = "[]";

    /// Serialized green card deck (JSON array of Card IDs).
    /// Stores remaining cards in deck.
    public string GreenCardDeckJson { get; set; } = "[]";

    /// Current round ID (nullable).
    public Guid? CurrentRoundId { get; set; }

    // Navigation properties

    /// Players in this game.
    public ICollection<PlayerEntity> Players { get; set; } = new List<PlayerEntity>();

    /// Rounds played in this game.
    public ICollection<RoundEntity> Rounds { get; set; } = new List<RoundEntity>();

    /// Current active round.
    public RoundEntity? CurrentRound { get; set; }
}