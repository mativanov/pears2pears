using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Infrastructure.Entities;

/// Entity Framework entity for persisting cards to the database.
/// Maps both RedCard and GreenCard domain entities to a single table (Table-Per-Hierarchy).
public class CardEntity
{
    /// Primary key.
    public Guid Id { get; set; }

    /// The text displayed on the card.
    public string Text { get; set; } = string.Empty;

    /// Type of card (Red or Green).
    public CardType Type { get; set; }

    /// Optional description (for RedCards) or synonyms (for GreenCards).
    public string? AdditionalInfo { get; set; }

    /// Timestamp when the card was created.
    public DateTime CreatedAt { get; set; }

    // Navigation properties

    /// Many-to-many relationship: Cards in players' hands.
    public ICollection<PlayerCardEntity> PlayerCards { get; set; } = new List<PlayerCardEntity>();

    /// Rounds where this card was the green card.
    public ICollection<RoundEntity> RoundsAsGreenCard { get; set; } = new List<RoundEntity>();
}