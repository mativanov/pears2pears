using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.Entities;

public class RedCard : Card
{
    public string? Description { get; private set; }

    /// Protected constructor for Entity Framework.
    protected RedCard() : base()
    {
        Type = CardType.Red;
    }

    public RedCard(string text) : base(text, CardType.Red)
    {
    }

    public RedCard(string text, string description) : base(text, CardType.Red)
    {
        Description = description?.Trim();
    }

    public override bool CanBePlayed(PlayerRole playerRole)
    {
        return playerRole == PlayerRole.Player;
    }

    public override string GetDisplayText()
    {
        if (string.IsNullOrWhiteSpace(Description))
            return Text;

        return $"{Text}\n{Description}";
    }

    public void SetDescription(string description)
    {
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description.Trim();
        }
    }

    /// Factory method to create a Red Card from raw data.
    public static RedCard Create(string text, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            return new RedCard(text);

        return new RedCard(text, description);
    }
}