namespace Pears2Pears.Domain.Entities;

public class Round
{
    public Guid Id { get; private set; }

    public Guid GameId { get; private set; }

    public int RoundNumber { get; private set; }

    public Guid JudgeId { get; private set; }

    public GreenCard GreenCard { get; private set; }

    private readonly Dictionary<Guid, RedCard> _playedCards;

    public IReadOnlyDictionary<Guid, RedCard> PlayedCards => _playedCards;

    public Guid? WinnerId { get; private set; }

    public RedCard? WinningCard { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? EndedAt { get; private set; }

    public bool IsCompleted => WinnerId.HasValue && EndedAt.HasValue;

    public bool AllCardsPlayed { get; private set; }

    /// Protected constructor for Entity Framework.
    protected Round()
    {
        Id = Guid.NewGuid();
        _playedCards = new Dictionary<Guid, RedCard>();
        GreenCard = null!;
        StartedAt = DateTime.UtcNow;
    }

    public Round(Guid gameId, int roundNumber, Guid judgeId, GreenCard greenCard)
    {
        if (roundNumber <= 0)
            throw new ArgumentException("Round number must be positive.", nameof(roundNumber));

        if (greenCard == null)
            throw new ArgumentNullException(nameof(greenCard));

        Id = Guid.NewGuid();
        GameId = gameId;
        RoundNumber = roundNumber;
        JudgeId = judgeId;
        GreenCard = greenCard;
        _playedCards = new Dictionary<Guid, RedCard>();
        StartedAt = DateTime.UtcNow;
        AllCardsPlayed = false;
    }

    public void PlayCard(Guid playerId, RedCard card)
    {
        if (card == null)
            throw new ArgumentNullException(nameof(card));

        if (IsCompleted)
            throw new InvalidOperationException("Cannot play card in a completed round.");

        if (playerId == JudgeId)
            throw new InvalidOperationException("Judge cannot play cards.");

        if (_playedCards.ContainsKey(playerId))
            throw new InvalidOperationException($"Player {playerId} has already played a card this round.");

        _playedCards[playerId] = card;
    }

    public bool HasPlayerPlayed(Guid playerId)
    {
        return _playedCards.ContainsKey(playerId);
    }

    public RedCard? GetPlayerCard(Guid playerId)
    {
        return _playedCards.GetValueOrDefault(playerId);
    }

    public void MarkAllCardsPlayed()
    {
        AllCardsPlayed = true;
    }

    public bool CheckAllCardsPlayed(int totalPlayers)
    {
        int expectedCards = totalPlayers - 1;
        return _playedCards.Count >= expectedCards;
    }

    public void SelectWinner(Guid winnerId, Guid judgingPlayerId)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Winner has already been selected for this round.");

        if (judgingPlayerId != JudgeId)
            throw new InvalidOperationException("Only the judge can select a winner.");

        if (winnerId == JudgeId)
            throw new InvalidOperationException("Judge cannot select themselves as winner.");

        if (!_playedCards.ContainsKey(winnerId))
            throw new InvalidOperationException($"Player {winnerId} did not play a card this round.");

        WinnerId = winnerId;
        WinningCard = _playedCards[winnerId];
        EndedAt = DateTime.UtcNow;
    }

    public IEnumerable<RedCard> GetShuffledCards()
    {
        var random = new Random();
        return _playedCards.Values.OrderBy(x => random.Next()).ToList();
    }

    public int GetPlayedCardCount()
    {
        return _playedCards.Count;
    }

    public TimeSpan GetDuration()
    {
        var endTime = EndedAt ?? DateTime.UtcNow;
        return endTime - StartedAt;
    }

    public bool IsValid()
    {
        return RoundNumber > 0
               && GreenCard != null
               && JudgeId != Guid.Empty
               && (!IsCompleted || (WinnerId.HasValue && WinningCard != null));
    }

    public void RemovePlayerCard(Guid playerId)
    {
        if (IsCompleted)
            throw new InvalidOperationException("Cannot remove cards from a completed round.");

        _playedCards.Remove(playerId);
        AllCardsPlayed = false;
    }

    public override string ToString()
    {
        var status = IsCompleted ? "Completed" : (AllCardsPlayed ? "Judging" : "Playing");
        return $"Round {RoundNumber} - {status} ({_playedCards.Count} cards played)";
    }

    // Equality based on Id (Entity pattern)
    public override bool Equals(object? obj)
    {
        if (obj is not Round other)
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