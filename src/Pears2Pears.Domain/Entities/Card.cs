using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.Entities;

/*
   Abstract base class representing a card in the game.
   Uses Template Method pattern for common card behavior.
*/
public abstract class Card
{
    public Guid Id { get; protected set; }
    public string Text { get; protected set; }
    public CardType Type { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    protected Card()
    {
        Id = Guid.NewGuid();
        Text = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    protected Card(string text, CardType type)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Card text cannot be empty.", nameof(text));

        Id = Guid.NewGuid();
        Text = text.Trim();
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }


    public abstract bool CanBePlayed(PlayerRole playerRole);

    public virtual string GetDisplayText()
    {
        return Text;
    }

    public virtual bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Text) && Text.Length <= 100;
    }

    public override string ToString()
    {
        return $"{Type} Card: {Text}";
    }

    // Equality based on Id (Entity pattern)
    public override bool Equals(object? obj)
    {
        if (obj is not Card other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}