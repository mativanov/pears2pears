using Pears2Pears.Domain.Enums;
using Pears2Pears.Domain.ValueObjects;

namespace Pears2Pears.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }

    public string Nickname { get; private set; }

    public Score Score { get; private set; }

    public PlayerRole Role { get; private set; }

    public Hand Hand { get; private set; }

    public Guid GameId { get; private set; }

    public bool IsConnected { get; private set; }

    public DateTime JoinedAt { get; private set; }

    public DateTime LastActivityAt { get; private set; }

    public bool IsHost { get; private set; }

    /// Protected constructor for Entity Framework.
    protected Player()
    {
        Id = Guid.NewGuid();
        Nickname = string.Empty;
        Score = Score.Zero;
        Role = PlayerRole.Player;
        Hand = new Hand();
        IsConnected = true;
        JoinedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
    }

    public Player(string nickname, Guid gameId, bool isHost = false)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ArgumentException("Nickname cannot be empty.", nameof(nickname));

        if (nickname.Length > 20)
            throw new ArgumentException("Nickname cannot exceed 20 characters.", nameof(nickname));

        Id = Guid.NewGuid();
        Nickname = nickname.Trim();
        Score = Score.Zero;
        Role = PlayerRole.Player;
        Hand = new Hand();
        GameId = gameId;
        IsConnected = true;
        IsHost = isHost;
        JoinedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
    }

    public void SetRole(PlayerRole role)
    {
        Role = role;
        UpdateActivity();
    }

    public void BecomeJudge()
    {
        if (Role == PlayerRole.Spectator)
            throw new InvalidOperationException("Spectators cannot become judges.");

        SetRole(PlayerRole.Judge);
    }

    public void BecomePlayer()
    {
        if (Role == PlayerRole.Spectator)
            throw new InvalidOperationException("Spectators cannot become players.");

        SetRole(PlayerRole.Player);
    }

    public bool IsJudge()
    {
        return Role == PlayerRole.Judge;
    }

    public bool CanPlayCards()
    {
        return Role == PlayerRole.Player && IsConnected;
    }

    public bool CanSelectWinner()
    {
        return Role == PlayerRole.Judge && IsConnected;
    }

    public RedCard PlayCard(Guid cardId)
    {
        if (!CanPlayCards())
            throw new InvalidOperationException($"Player '{Nickname}' cannot play cards (Role: {Role}).");

        var card = Hand.PlayCard(cardId);
        UpdateActivity();

        return card;
    }

    public void DrawCards(IEnumerable<RedCard> cards)
    {
        Hand.AddCards(cards);
        UpdateActivity();
    }

    public void DrawCard(RedCard card)
    {
        Hand.AddCard(card);
        UpdateActivity();
    }

    public void AwardPoint()
    {
        Score = Score.Increment();
        UpdateActivity();
    }

    public bool HasWon(int winningScore = Score.StandardWinningScore)
    {
        return Score.HasReachedWinningScore(winningScore);
    }

    public void Disconnect()
    {
        IsConnected = false;
        UpdateActivity();
    }

    public void Reconnect()
    {
        IsConnected = true;
        UpdateActivity();
    }

    public void UpdateActivity()
    {
        LastActivityAt = DateTime.UtcNow;
    }

    public void ChangeNickname(string newNickname)
    {
        if (string.IsNullOrWhiteSpace(newNickname))
            throw new ArgumentException("Nickname cannot be empty.", nameof(newNickname));

        if (newNickname.Length > 20)
            throw new ArgumentException("Nickname cannot exceed 20 characters.", nameof(newNickname));

        Nickname = newNickname.Trim();
        UpdateActivity();
    }

    public bool IsInactive(TimeSpan timeout)
    {
        return DateTime.UtcNow - LastActivityAt > timeout;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Nickname)
               && Hand.IsValid()
               && Score != null;
    }

    public void ClearHand()
    {
        Hand.Clear();
        UpdateActivity();
    }

    public override string ToString()
    {
        return $"Player '{Nickname}' (Role: {Role}, Score: {Score}, Cards: {Hand.CardCount})";
    }

    // Equality based on Id (Entity pattern)
    public override bool Equals(object? obj)
    {
        if (obj is not Player other)
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