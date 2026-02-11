using Pears2Pears.Domain.ValueObjects;
using Xunit;
using FluentAssertions;

namespace Pears2Pears.Tests.ValueObjects;

public class GameCodeTests
{
    [Fact]
    public void Generate_ShouldCreateValidGameCode()
    {
        // Act
        var code = GameCode.Generate();

        // Assert
        code.Should().NotBeNull();
        code.Value.Should().HaveLength(6);
        GameCode.IsValid(code.Value).Should().BeTrue();
    }

    [Theory]
    [InlineData("ABC234")]
    [InlineData("XYZ789")]
    [InlineData("QWERTY")]
    public void From_WithValidCode_ShouldCreateGameCode(string validCode)
    {
        // Act
        var code = GameCode.From(validCode);

        // Assert
        code.Should().NotBeNull();
        code.Value.Should().Be(validCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("AB")]
    [InlineData("ABCDEFGH")]
    [InlineData("ABC IO1")] // Contains invalid chars (I, O, space)
    public void From_WithInvalidCode_ShouldThrowException(string invalidCode)
    {
        // Act & Assert
        Action act = () => GameCode.From(invalidCode);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var code1 = GameCode.From("ABC234");
        var code2 = GameCode.From("ABC234");

        // Assert
        code1.Should().Be(code2);
        (code1 == code2).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var code1 = GameCode.From("ABC234");
        var code2 = GameCode.From("XYZ789");

        // Assert
        code1.Should().NotBe(code2);
        (code1 != code2).Should().BeTrue();
    }
}