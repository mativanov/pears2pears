namespace Pears2Pears.Domain.ValueObjects;

/// Value Object representing a unique 6-character game code.
/// Immutable and validated upon creation.
public sealed class GameCode : IEquatable<GameCode>
{
    private const int CodeLength = 6;
    private const string AllowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excluded: I, O, 0, 1 (confusing)

    public string Value { get; }

    /// Private constructor to enforce validation through factory methods.
    private GameCode(string value)
    {
        Value = value;
    }

    /// Creates a GameCode from an existing code string.
    /// Validates format and characters.
    public static GameCode From(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Game code cannot be empty.", nameof(code));

        var normalized = code.Trim().ToUpperInvariant();

        if (normalized.Length != CodeLength)
            throw new ArgumentException($"Game code must be exactly {CodeLength} characters.", nameof(code));

        if (!normalized.All(c => AllowedCharacters.Contains(c)))
            throw new ArgumentException("Game code contains invalid characters.", nameof(code));

        return new GameCode(normalized);
    }

    /// Generates a new random 6-character game code.
    public static GameCode Generate()
    {
        var random = new Random();
        var code = new char[CodeLength];

        for (int i = 0; i < CodeLength; i++)
        {
            code[i] = AllowedCharacters[random.Next(AllowedCharacters.Length)];
        }

        return new GameCode(new string(code));
    }

    /// Validates if a string is a valid game code format.
    public static bool IsValid(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        var normalized = code.Trim().ToUpperInvariant();

        return normalized.Length == CodeLength 
               && normalized.All(c => AllowedCharacters.Contains(c));
    }

    // Equality implementation (Value Object pattern)
    public bool Equals(GameCode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is GameCode other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    // Operators
    public static bool operator ==(GameCode? left, GameCode? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(GameCode? left, GameCode? right)
    {
        return !Equals(left, right);
    }

    // Implicit conversion to string for convenience
    public static implicit operator string(GameCode gameCode) => gameCode.Value;
}