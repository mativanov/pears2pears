namespace Pears2Pears.Domain.ValueObjects;

/// Value Object representing a player's score.
/// Immutable and validated to ensure non-negative values.
public sealed class Score : IEquatable<Score>, IComparable<Score>
{
    public int Value { get; }

    /// Default winning score for standard game mode.
    public const int StandardWinningScore = 7;

    /// Winning score for fast game mode.
    public const int FastGameWinningScore = 4;

    /// Zero score - starting value for all players.
    public static readonly Score Zero = new(0);

    /// Private constructor to enforce validation.
    private Score(int value)
    {
        Value = value;
    }

    /// Creates a Score from an integer value.
    public static Score From(int value)
    {
        if (value < 0)
            throw new ArgumentException("Score cannot be negative.", nameof(value));

        return new Score(value);
    }

    /// Increments the score by one point (player wins a round).
    public Score Increment()
    {
        return new Score(Value + 1);
    }

    /// Increments the score by a specified amount.
    public Score Add(int points)
    {
        if (points < 0)
            throw new ArgumentException("Cannot add negative points.", nameof(points));

        return new Score(Value + points);
    }

    /// Checks if this score has reached the winning threshold.
    public bool HasReachedWinningScore(int winningScore = StandardWinningScore)
    {
        return Value >= winningScore;
    }

    /// Checks if this score is higher than another score.
    public bool IsHigherThan(Score other)
    {
        return Value > other.Value;
    }

    // Equality implementation (Value Object pattern)
    public bool Equals(Score? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Score other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    // Comparison implementation
    public int CompareTo(Score? other)
    {
        if (other is null) return 1;
        return Value.CompareTo(other.Value);
    }

    // Operators
    public static bool operator ==(Score? left, Score? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Score? left, Score? right)
    {
        return !Equals(left, right);
    }

    public static bool operator >(Score left, Score right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <(Score left, Score right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >=(Score left, Score right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator <=(Score left, Score right)
    {
        return left.Value <= right.Value;
    }

    // Implicit conversion to int for convenience
    public static implicit operator int(Score score) => score.Value;
}