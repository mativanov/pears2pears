namespace Pears2Pears.Infrastructure.Entities;

/// Join table entity for many-to-many relationship between Players and Cards.
/// Represents cards currently in a player's hand.
public class PlayerCardEntity
{
    /// Foreign key to the player.
    public Guid PlayerId { get; set; }

    /// Foreign key to the card.
    public Guid CardId { get; set; }

    /// When the card was added to the player's hand.
    public DateTime AddedAt { get; set; }

    /// Order of the card in player's hand (for UI display).
    public int OrderInHand { get; set; }

    // Navigation properties

    /// The player who has this card.
    public PlayerEntity Player { get; set; } = null!;

    /// The card in the player's hand.
    public CardEntity Card { get; set; } = null!;
}