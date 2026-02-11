namespace Pears2Pears.Domain.Entities;

public class Hand
{
    public const int MaxCardCount = 7;

    public const int MinCardCount = 0;

    private readonly List<RedCard> _cards;

    public IReadOnlyCollection<RedCard> Cards => _cards.AsReadOnly();

    public int CardCount => _cards.Count;

    public bool IsFull => CardCount >= MaxCardCount;

    public bool IsEmpty => CardCount == 0;

    public Hand()
    {
        _cards = new List<RedCard>();
    }

    public Hand(IEnumerable<RedCard> initialCards)
    {
        _cards = new List<RedCard>(initialCards);
        
        if (_cards.Count > MaxCardCount)
            throw new InvalidOperationException($"Cannot create hand with more than {MaxCardCount} cards.");
    }

    public void AddCard(RedCard card)
    {
        if (card == null)
            throw new ArgumentNullException(nameof(card));

        if (IsFull)
            throw new InvalidOperationException($"Cannot add card. Hand is full (max {MaxCardCount} cards).");

        if (_cards.Any(c => c.Id == card.Id))
            throw new InvalidOperationException("Card already exists in hand.");

        _cards.Add(card);
    }

    public void AddCards(IEnumerable<RedCard> cards)
    {
        foreach (var card in cards)
        {
            AddCard(card);
        }
    }

    public RedCard? RemoveCard(Guid cardId)
    {
        var card = _cards.FirstOrDefault(c => c.Id == cardId);
        
        if (card != null)
        {
            _cards.Remove(card);
        }

        return card;
    }

    public RedCard PlayCard(Guid cardId)
    {
        var card = RemoveCard(cardId);

        if (card == null)
            throw new InvalidOperationException($"Card with ID {cardId} not found in hand.");

        return card;
    }

    public bool HasCard(Guid cardId)
    {
        return _cards.Any(c => c.Id == cardId);
    }

    public RedCard? GetCard(Guid cardId)
    {
        return _cards.FirstOrDefault(c => c.Id == cardId);
    }

    public int GetCardsNeededToFill()
    {
        return MaxCardCount - CardCount;
    }

    /// Clears all cards from the hand.
    /// Used when player leaves game or game ends.
    public void Clear()
    {
        _cards.Clear();
    }

    public bool IsValid()
    {
        return CardCount >= MinCardCount 
               && CardCount <= MaxCardCount
               && _cards.All(c => c != null && c.Type == Enums.CardType.Red);
    }

    public List<RedCard> GetCardsInOrder()
    {
        return new List<RedCard>(_cards);
    }

    public override string ToString()
    {
        return $"Hand: {CardCount}/{MaxCardCount} cards";
    }
}