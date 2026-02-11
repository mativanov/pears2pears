using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.Entities;

public class GreenCard : Card
{
    public string? Synonyms { get; private set; }

    protected GreenCard() : base()
    {
        Type = CardType.Green;
    }

    public GreenCard(string text) : base(text, CardType.Green)
    {
    }

    public GreenCard(string text, string synonyms) : base(text, CardType.Green)
    {
        Synonyms = synonyms?.Trim();
    }

    public override bool CanBePlayed(PlayerRole playerRole)
    {
        return playerRole == PlayerRole.Judge;
    }

    public override string GetDisplayText()
    {
        if (string.IsNullOrWhiteSpace(Synonyms))
            return Text;

        return $"{Text}\n({Synonyms})";
    }

    public void SetSynonyms(string synonyms)
    {
        if (!string.IsNullOrWhiteSpace(synonyms))
        {
            Synonyms = synonyms.Trim();
        }
    }

    /// Factory method to create a Green Card from raw data.
    public static GreenCard Create(string text, string? synonyms = null)
    {
        if (string.IsNullOrWhiteSpace(synonyms))
            return new GreenCard(text);

        return new GreenCard(text, synonyms);
    }

    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // Green cards should be relatively short (adjectives)
        return Text.Length <= 50;
    }
}