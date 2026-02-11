using Pears2Pears.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace Pears2Pears.Tests.ValueObjects;

public class ScoreTests
{
    [Fact]
    public void Zero_ShouldHaveValueOfZero()
    {
        // Assert
        Score.Zero.Value.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void From_WithValidValue_ShouldCreateScore(int value)
    {
        // Act
        var score = Score.From(value);

        // Assert
        score.Value.Should().Be(value);
    }

    [Fact]
    public void From_WithNegativeValue_ShouldThrowException()
    {
        // Act & Assert
        Action act = () => Score.From(-1);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Increment_ShouldAddOnePoint()
    {
        // Arrange
        var score = Score.From(5);

        // Act
        var newScore = score.Increment();

        // Assert
        newScore.Value.Should().Be(6);
        score.Value.Should().Be(5); // Original should be unchanged (immutable)
    }

    [Fact]
    public void Add_WithValidPoints_ShouldAddPoints()
    {
        // Arrange
        var score = Score.From(3);

        // Act
        var newScore = score.Add(4);

        // Assert
        newScore.Value.Should().Be(7);
    }

    [Fact]
    public void HasReachedWinningScore_WithStandardWin_ShouldReturnTrue()
    {
        // Arrange
        var score = Score.From(7);

        // Act & Assert
        score.HasReachedWinningScore().Should().BeTrue();
    }

    [Fact]
    public void HasReachedWinningScore_BelowThreshold_ShouldReturnFalse()
    {
        // Arrange
        var score = Score.From(6);

        // Act & Assert
        score.HasReachedWinningScore().Should().BeFalse();
    }

    [Fact]
    public void IsHigherThan_WithHigherScore_ShouldReturnTrue()
    {
        // Arrange
        var score1 = Score.From(10);
        var score2 = Score.From(5);

        // Act & Assert
        score1.IsHigherThan(score2).Should().BeTrue();
    }

    [Fact]
    public void Comparison_ShouldWorkCorrectly()
    {
        // Arrange
        var score1 = Score.From(5);
        var score2 = Score.From(10);

        // Assert
        (score1 < score2).Should().BeTrue();
        (score2 > score1).Should().BeTrue();
    }
}