using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;
using Xunit;
using FluentAssertions;

namespace Pears2Pears.Tests.Entities;

public class GameTests
{
    [Fact]
    public void Constructor_ShouldCreateGameInWaitingForPlayersState()
    {
        // Act
        var game = new Game("HostPlayer");

        // Assert
        game.Should().NotBeNull();
        game.Status.Should().Be(GameStatus.WaitingForPlayers);
        game.Players.Should().HaveCount(1); // Host player
        game.Code.Should().NotBeNull();
    }

    [Fact]
    public void AddPlayer_WithValidNickname_ShouldAddPlayer()
    {
        // Arrange
        var game = new Game("Host");

        // Act
        game.AddPlayer("Player2");

        // Assert
        game.Players.Should().HaveCount(2);
    }

    [Fact]
    public void AddPlayer_WithDuplicateNickname_ShouldThrowException()
    {
        // Arrange
        var game = new Game("Host");
        game.AddPlayer("Player2");

        // Act & Assert
        Action act = () => game.AddPlayer("Player2");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public void AddPlayer_WhenFull_ShouldThrowException()
    {
        // Arrange
        var game = new Game("Host");
        for (int i = 1; i < Game.MaxPlayers; i++)
        {
            game.AddPlayer($"Player{i}");
        }

        // Act & Assert
        Action act = () => game.AddPlayer("ExtraPlayer");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*full*");
    }

    [Fact]
    public void StartGame_WithMinimumPlayers_ShouldChangeStatus()
    {
        // Arrange
        var game = new Game("Host");
        game.AddPlayer("Player2");
        game.AddPlayer("Player3");
        game.AddPlayer("Player4");

        // Initialize decks
        var redCards = Enumerable.Range(1, 50)
            .Select(i => new RedCard($"Red Card {i}"))
            .ToList();
        var greenCards = Enumerable.Range(1, 20)
            .Select(i => new GreenCard($"Green Card {i}"))
            .ToList();
        game.InitializeDecks(redCards, greenCards);

        // Act
        game.StartGame();

        // Assert
        game.Status.Should().Be(GameStatus.InProgress);
        game.StartedAt.Should().NotBeNull();
        game.CurrentRound.Should().NotBeNull();
    }

    [Fact]
    public void StartGame_WithoutMinimumPlayers_ShouldThrowException()
    {
        // Arrange
        var game = new Game("Host");
        game.AddPlayer("Player2");

        // Act & Assert
        Action act = () => game.StartGame();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*at least*");
    }

    [Fact]
    public void GetCurrentJudge_ShouldReturnJudgePlayer()
    {
        // Arrange
        var game = new Game("Host");
        game.AddPlayer("Player2");
        game.AddPlayer("Player3");
        game.AddPlayer("Player4");

        var redCards = Enumerable.Range(1, 50).Select(i => new RedCard($"Red {i}")).ToList();
        var greenCards = Enumerable.Range(1, 20).Select(i => new GreenCard($"Green {i}")).ToList();
        game.InitializeDecks(redCards, greenCards);
        
        game.StartGame();

        // Act
        var judge = game.GetCurrentJudge();

        // Assert
        judge.Should().NotBeNull();
        judge!.IsJudge().Should().BeTrue();
    }

    [Fact]
    public void PlayCard_WhenValid_ShouldAddCardToRound()
    {
        // Arrange
        var game = CreateGameWithFourPlayersAndStart();
        var nonJudgePlayers = game.Players.Where(p => !p.IsJudge()).ToList();
        var player = nonJudgePlayers.First();
        var cardToPlay = player.Hand.Cards.First();

        // Act
        game.PlayCard(player.Id, cardToPlay.Id);

        // Assert
        game.CurrentRound!.PlayedCards.Should().ContainKey(player.Id);
    }

    [Fact]
    public void PlayCard_WhenJudge_ShouldThrowException()
    {
        // Arrange
        var game = CreateGameWithFourPlayersAndStart();
        var judge = game.GetCurrentJudge()!;

        // Act & Assert
        Action act = () => game.PlayCard(judge.Id, Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot play cards*");
    }

    [Fact]
    public void SelectWinner_WhenAllCardsPlayed_ShouldAwardPoint()
    {
        // Arrange
        var game = CreateGameWithFourPlayersAndStart();
        var judge = game.GetCurrentJudge()!;
        var nonJudgePlayers = game.Players.Where(p => !p.IsJudge()).ToList();

        // All non-judges play cards
        foreach (var player in nonJudgePlayers)
        {
            var card = player.Hand.Cards.First();
            game.PlayCard(player.Id, card.Id);
        }

        var winner = nonJudgePlayers.First();
        var initialScore = winner.Score.Value;

        // Act
        game.SelectWinner(judge.Id, winner.Id);

        // Assert
        winner.Score.Value.Should().Be(initialScore + 1);
    }

    [Fact]
    public void GetLeaderboard_ShouldReturnPlayersOrderedByScore()
    {
        // Arrange
        var game = new Game("Host");
        game.AddPlayer("Player2");
        game.AddPlayer("Player3");

        var players = game.Players.ToList();
        players[0].AwardPoint();
        players[0].AwardPoint();
        players[1].AwardPoint();

        // Act
        var leaderboard = game.GetLeaderboard();

        // Assert
        leaderboard.Should().HaveCount(3);
        leaderboard[0].Score.Value.Should().BeGreaterThanOrEqualTo(leaderboard[1].Score.Value);
        leaderboard[1].Score.Value.Should().BeGreaterThanOrEqualTo(leaderboard[2].Score.Value);
    }

    // Helper method
    private Game CreateGameWithFourPlayersAndStart()
    {
        var game = new Game("Host");
        game.AddPlayer("Player2");
        game.AddPlayer("Player3");
        game.AddPlayer("Player4");

        var redCards = Enumerable.Range(1, 50).Select(i => new RedCard($"Red {i}")).ToList();
        var greenCards = Enumerable.Range(1, 20).Select(i => new GreenCard($"Green {i}")).ToList();
        game.InitializeDecks(redCards, greenCards);

        game.StartGame();
        return game;
    }
}