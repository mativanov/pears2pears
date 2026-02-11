using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;
using Pears2Pears.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace Pears2Pears.Tests.Entities;

public class PlayerTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePlayer()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var nickname = "TestPlayer";

        // Act
        var player = new Player(nickname, gameId);

        // Assert
        player.Should().NotBeNull();
        player.Nickname.Should().Be(nickname);
        player.GameId.Should().Be(gameId);
        player.Score.Should().Be(Score.Zero);
        player.Role.Should().Be(PlayerRole.Player);
        player.IsConnected.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidNickname_ShouldThrowException(string invalidNickname)
    {
        // Arrange
        var gameId = Guid.NewGuid();

        // Act & Assert
        Action act = () => new Player(invalidNickname, gameId);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNullNickname_ShouldThrowException()
    {
        // Arrange
        var gameId = Guid.NewGuid();

        // Act & Assert
        Action act = () => new Player(null!, gameId);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BecomeJudge_ShouldChangeRoleToJudge()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());

        // Act
        player.BecomeJudge();

        // Assert
        player.IsJudge().Should().BeTrue();
        player.Role.Should().Be(PlayerRole.Judge);
    }

    [Fact]
    public void BecomePlayer_WhenJudge_ShouldChangeRoleToPlayer()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        player.BecomeJudge();

        // Act
        player.BecomePlayer();

        // Assert
        player.IsJudge().Should().BeFalse();
        player.Role.Should().Be(PlayerRole.Player);
    }

    [Fact]
    public void AwardPoint_ShouldIncrementScore()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        var initialScore = player.Score.Value;

        // Act
        player.AwardPoint();

        // Assert
        player.Score.Value.Should().Be(initialScore + 1);
    }

    [Fact]
    public void HasWon_WithWinningScore_ShouldReturnTrue()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        
        // Act - Award 7 points
        for (int i = 0; i < 7; i++)
        {
            player.AwardPoint();
        }

        // Assert
        player.HasWon().Should().BeTrue();
    }

    [Fact]
    public void CanPlayCards_WhenPlayer_ShouldReturnTrue()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());

        // Assert
        player.CanPlayCards().Should().BeTrue();
    }

    [Fact]
    public void CanPlayCards_WhenJudge_ShouldReturnFalse()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        player.BecomeJudge();

        // Assert
        player.CanPlayCards().Should().BeFalse();
    }

    [Fact]
    public void CanSelectWinner_WhenJudge_ShouldReturnTrue()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        player.BecomeJudge();

        // Assert
        player.CanSelectWinner().Should().BeTrue();
    }

    [Fact]
    public void DrawCard_ShouldAddCardToHand()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        var card = new RedCard("Test Card");

        // Act
        player.DrawCard(card);

        // Assert
        player.Hand.CardCount.Should().Be(1);
        player.Hand.HasCard(card.Id).Should().BeTrue();
    }

    [Fact]
    public void PlayCard_WhenPlayer_ShouldRemoveCardFromHand()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        var card = new RedCard("Test Card");
        player.DrawCard(card);

        // Act
        var playedCard = player.PlayCard(card.Id);

        // Assert
        playedCard.Should().Be(card);
        player.Hand.CardCount.Should().Be(0);
    }

    [Fact]
    public void PlayCard_WhenJudge_ShouldThrowException()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        var card = new RedCard("Test Card");
        player.DrawCard(card);
        player.BecomeJudge();

        // Act & Assert
        Action act = () => player.PlayCard(card.Id);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot play cards*");
    }

    [Fact]
    public void Disconnect_ShouldSetIsConnectedToFalse()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());

        // Act
        player.Disconnect();

        // Assert
        player.IsConnected.Should().BeFalse();
    }

    [Fact]
    public void Reconnect_ShouldSetIsConnectedToTrue()
    {
        // Arrange
        var player = new Player("TestPlayer", Guid.NewGuid());
        player.Disconnect();

        // Act
        player.Reconnect();

        // Assert
        player.IsConnected.Should().BeTrue();
    }
}