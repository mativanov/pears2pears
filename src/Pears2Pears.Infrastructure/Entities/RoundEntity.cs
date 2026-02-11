namespace Pears2Pears.Infrastructure.Entities;

/// Entity Framework entity for persisting rounds to the database.
public class RoundEntity
{
    /// Primary key.
    public Guid Id { get; set; }

    /// Foreign key to the game.
    public Guid GameId { get; set; }

    /// Round number (1-based).
    public int RoundNumber { get; set; }

    /// Foreign key to the judge player.
    public Guid JudgeId { get; set; }

    /// Foreign key to the green card for this round.
    public Guid GreenCardId { get; set; }

    /// Foreign key to the winner player (nullable).
    public Guid? WinnerId { get; set; }

    /// Foreign key to the winning card (nullable).
    public Guid? WinningCardId { get; set; }

    /// Timestamp when round started.
    public DateTime StartedAt { get; set; }

    /// Timestamp when round ended (nullable).
    public DateTime? EndedAt { get; set; }

    /// Are all cards played.
    public bool AllCardsPlayed { get; set; }

    /// Serialized JSON of played cards: { "playerId": "cardId" }
    /// Stored as JSON string for simplicity.
    public string PlayedCardsJson { get; set; } = "{}";

    // Navigation properties

    /// The game this round belongs to.
    public GameEntity Game { get; set; } = null!;

    /// The judge for this round.
    public PlayerEntity Judge { get; set; } = null!;

    /// The green card for this round.
    public CardEntity GreenCard { get; set; } = null!;

    /// The winner of this round (nullable).
    public PlayerEntity? Winner { get; set; }

    /// The winning card (nullable).
    public CardEntity? WinningCard { get; set; }
}