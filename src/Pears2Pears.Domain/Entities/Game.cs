using Pears2Pears.Domain.Enums;
using Pears2Pears.Domain.States;
using Pears2Pears.Domain.ValueObjects;

namespace Pears2Pears.Domain.Entities;

/*
    Aggregate Root representing a game session.
    Uses State Pattern to manage game flow and enforce business rules.
    Implements pessimistic locking through state transitions (FR6).
*/
public class Game
{
    public Guid Id { get; private set; }

    public GameCode Code { get; private set; }

    /// Current state of the game (uses State Pattern).
    private IGameState _currentState;

    public GameStatus Status => _currentState.Status;

    
    private readonly List<Player> _players;
    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    private readonly List<Round> _rounds;
    public IReadOnlyCollection<Round> Rounds => _rounds.AsReadOnly();

    public Round? CurrentRound { get; private set; }

    private readonly List<RedCard> _redCardDeck;

    private readonly List<GreenCard> _greenCardDeck;

    public int WinningScore { get; private set; }

    public Guid? WinnerId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? EndedAt { get; private set; }

    public const int MinPlayers = 4;
    public const int MaxPlayers = 8;
    public const int CardsPerHand = 7;

    /// Protected constructor for Entity Framework.
    protected Game()
    {
        Id = Guid.NewGuid();
        Code = GameCode.Generate();
        _currentState = new WaitingForPlayersState();
        _players = new List<Player>();
        _rounds = new List<Round>();
        _redCardDeck = new List<RedCard>();
        _greenCardDeck = new List<GreenCard>();
        WinningScore = Score.StandardWinningScore;
        CreatedAt = DateTime.UtcNow;
    }

    public Game(string hostNickname, int winningScore = Score.StandardWinningScore)
    {
        if (string.IsNullOrWhiteSpace(hostNickname))
            throw new ArgumentException("Host nickname cannot be empty.", nameof(hostNickname));

        if (winningScore < 1 || winningScore > 20)
            throw new ArgumentException("Winning score must be between 1 and 20.", nameof(winningScore));

        Id = Guid.NewGuid();
        Code = GameCode.Generate();
        _currentState = new WaitingForPlayersState();
        _players = new List<Player>();
        _rounds = new List<Round>();
        _redCardDeck = new List<RedCard>();
        _greenCardDeck = new List<GreenCard>();
        WinningScore = winningScore;
        CreatedAt = DateTime.UtcNow;

        // Add host as first player
        var host = new Player(hostNickname, Id, isHost: true);
        _players.Add(host);
    }

    #region Player Management

    public void AddPlayer(string nickname)
    {
        if (!_currentState.CanPlayerJoin())
            throw new InvalidOperationException($"Cannot join game in {Status} state.");

        if (_players.Count >= MaxPlayers)
            throw new InvalidOperationException($"Game is full (max {MaxPlayers} players).");

        if (_players.Any(p => p.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Player with nickname '{nickname}' already exists.");

        var player = new Player(nickname, Id);
        _players.Add(player);
    }

    public void RemovePlayer(Guid playerId)
    {
        if (!_currentState.CanPlayerLeave())
            throw new InvalidOperationException($"Cannot leave game in {Status} state.");

        var player = GetPlayer(playerId);
        if (player == null)
            throw new InvalidOperationException($"Player {playerId} not found.");

        _players.Remove(player);

        // If player was in current round, remove their card
        if (CurrentRound != null && CurrentRound.HasPlayerPlayed(playerId))
        {
            CurrentRound.RemovePlayerCard(playerId);
        }
    }

    public Player? GetPlayer(Guid playerId)
    {
        return _players.FirstOrDefault(p => p.Id == playerId);
    }

    public Player? GetCurrentJudge()
    {
        return _players.FirstOrDefault(p => p.IsJudge());
    }

    #endregion

    #region Game Flow

    /// Starts the game if conditions are met.
    /// Transitions: WaitingForPlayersState → PlayingCardsState
    public void StartGame()
    {
        if (!_currentState.CanStartGame())
            throw new InvalidOperationException($"Cannot start game in {Status} state.");

        if (_players.Count < MinPlayers)
            throw new InvalidOperationException($"Need at least {MinPlayers} players to start (current: {_players.Count}).");

        if (_redCardDeck.Count == 0 || _greenCardDeck.Count == 0)
            throw new InvalidOperationException("Card decks must be initialized before starting.");

        _currentState.OnExit();
        _currentState = new PlayingCardsState();
        _currentState.OnEnter();

        StartedAt = DateTime.UtcNow;

        // Deal initial cards to all players
        DealInitialCards();

        // Start first round
        StartNewRound();
    }

    /// Starts a new round with the next judge.
    public void StartNewRound()
    {
        if (!_currentState.CanStartNewRound() && Status != GameStatus.InProgress)
            throw new InvalidOperationException($"Cannot start new round in {Status} state.");

        RotateJudge();

        var judge = GetCurrentJudge();
        if (judge == null)
            throw new InvalidOperationException("No judge assigned.");

        // Draw green card
        var greenCard = DrawGreenCard();
        if (greenCard == null)
            throw new InvalidOperationException("No green cards available.");

        // Create new round
        var roundNumber = _rounds.Count + 1;
        CurrentRound = new Round(Id, roundNumber, judge.Id, greenCard);
        _rounds.Add(CurrentRound);

        // Transition to PlayingCardsState
        if (_currentState is not PlayingCardsState)
        {
            _currentState.OnExit();
            _currentState = new PlayingCardsState();
            _currentState.OnEnter();
        }
    }

    /// Player plays a card in the current round.
    public void PlayCard(Guid playerId, Guid cardId)
    {
        if (!_currentState.CanPlayCard())
            throw new InvalidOperationException($"Cannot play cards in {Status} state.");

        if (CurrentRound == null)
            throw new InvalidOperationException("No active round.");

        var player = GetPlayer(playerId);
        if (player == null)
            throw new InvalidOperationException($"Player {playerId} not found.");

        if (!player.CanPlayCards())
            throw new InvalidOperationException($"Player {player.Nickname} cannot play cards (Role: {player.Role}).");

        if (CurrentRound.HasPlayerPlayed(playerId))
            throw new InvalidOperationException($"Player {player.Nickname} has already played a card this round.");

        // Play card from player's hand
        var card = player.PlayCard(cardId);
        CurrentRound.PlayCard(playerId, card);

        // Check if all cards have been played
        CheckAllCardsPlayed();
    }

    /// Judge selects the winning card.
    /// Implements FR6: Pessimistic locking - only judge can select.
    public void SelectWinner(Guid judgingPlayerId, Guid winningPlayerId)
    {
        if (!_currentState.CanSelectWinner())
            throw new InvalidOperationException($"Cannot select winner in current state.");

        if (CurrentRound == null)
            throw new InvalidOperationException("No active round.");

        var judge = GetPlayer(judgingPlayerId);
        if (judge == null || !judge.CanSelectWinner())
            throw new InvalidOperationException("Only the judge can select a winner.");

        CurrentRound.SelectWinner(winningPlayerId, judgingPlayerId);

        // Award point to winner
        var winner = GetPlayer(winningPlayerId);
        if (winner != null)
        {
            winner.AwardPoint();

            // Check if winner has won the game
            if (winner.HasWon(WinningScore))
            {
                EndGame(winningPlayerId);
                return;
            }
        }

        // Transition to RoundEndState
        _currentState.OnExit();
        _currentState = new RoundEndState();
        _currentState.OnEnter();

        // Refill winner's hand
        if (winner != null)
        {
            RefillPlayerHand(winner);
        }
    }

    /// Ends the game with a winner.
    private void EndGame(Guid winnerId)
    {
        WinnerId = winnerId;
        EndedAt = DateTime.UtcNow;

        _currentState.OnExit();
        _currentState = new GameOverState();
        _currentState.OnEnter();
    }

    #endregion

    #region Card Management

    public void InitializeDecks(IEnumerable<RedCard> redCards, IEnumerable<GreenCard> greenCards)
    {
        _redCardDeck.Clear();
        _greenCardDeck.Clear();

        _redCardDeck.AddRange(redCards);
        _greenCardDeck.AddRange(greenCards);

        ShuffleDecks();
    }

    private void ShuffleDecks()
    {
        var random = new Random();
        _redCardDeck.OrderBy(x => random.Next()).ToList();
        _greenCardDeck.OrderBy(x => random.Next()).ToList();
    }

    private void DealInitialCards()
    {
        foreach (var player in _players)
        {
            var cards = DrawRedCards(CardsPerHand);
            player.DrawCards(cards);
        }
    }

    private List<RedCard> DrawRedCards(int count)
    {
        var cards = new List<RedCard>();

        for (int i = 0; i < count && _redCardDeck.Count > 0; i++)
        {
            var card = _redCardDeck[0];
            _redCardDeck.RemoveAt(0);
            cards.Add(card);
        }

        return cards;
    }

    private GreenCard? DrawGreenCard()
    {
        if (_greenCardDeck.Count == 0)
            return null;

        var card = _greenCardDeck[0];
        _greenCardDeck.RemoveAt(0);
        return card;
    }

    private void RefillPlayerHand(Player player)
    {
        var cardsNeeded = player.Hand.GetCardsNeededToFill();
        if (cardsNeeded > 0)
        {
            var cards = DrawRedCards(cardsNeeded);
            player.DrawCards(cards);
        }
    }

    #endregion

    #region Judge Rotation

    /// Rotates the judge role to the next player.
    /// Implements FR5: Automatic judge rotation.
    private void RotateJudge()
    {
        var currentJudge = GetCurrentJudge();

        // If no judge yet, assign first player
        if (currentJudge == null)
        {
            _players[0].BecomeJudge();
            return;
        }

        currentJudge.BecomePlayer();

        // Find next player
        var currentIndex = _players.IndexOf(currentJudge);
        var nextIndex = (currentIndex + 1) % _players.Count;
        _players[nextIndex].BecomeJudge();
    }

    #endregion

    #region State Checks

    /// Checks if all non-judge players have played their cards.
    /// If yes, transitions to JudgingState.
    private void CheckAllCardsPlayed()
    {
        if (CurrentRound == null)
            return;

        var nonJudgeCount = _players.Count(p => !p.IsJudge());
        
        if (CurrentRound.CheckAllCardsPlayed(_players.Count))
        {
            CurrentRound.MarkAllCardsPlayed();

            // Transition to JudgingState
            _currentState.OnExit();
            _currentState = new JudgingState();
            _currentState.OnEnter();
        }
    }

    public Player? GetWinner()
    {
        if (WinnerId == null)
            return null;

        return GetPlayer(WinnerId.Value);
    }

    public List<Player> GetLeaderboard()
    {
        return _players.OrderByDescending(p => p.Score).ToList();
    }

    #endregion

    #region Validation

    /// Validates the game state.
    public bool IsValid()
    {
        return Code != null
               && _players.Count >= 0
               && (Status != GameStatus.InProgress || _players.Count >= MinPlayers)
               && _players.All(p => p.IsValid());
    }

    #endregion

    public override string ToString()
    {
        return $"Game {Code} - {Status} ({_players.Count} players, Round {_rounds.Count})";
    }

    // Equality based on Id (Entity pattern)
    public override bool Equals(object? obj)
    {
        if (obj is not Game other)
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