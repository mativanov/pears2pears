using Pears2Pears.Domain.Entities;

namespace Pears2Pears.Domain.Interfaces;

/// Repository interface for Round entity.
/// Defines contract for data access operations on rounds.
public interface IRoundRepository
{
    /// Gets a round by ID.
    Task<Round?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets a round with all related data (cards, players).
    Task<Round?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets all rounds for a specific game.
    Task<IEnumerable<Round>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets rounds for a game ordered by round number.
    Task<IEnumerable<Round>> GetByGameIdOrderedAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets the current (latest) round for a game.
    Task<Round?> GetCurrentRoundByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets a specific round by game ID and round number.
    Task<Round?> GetByGameIdAndRoundNumberAsync(Guid gameId, int roundNumber, CancellationToken cancellationToken = default);

    /// Gets completed rounds for a game.
    Task<IEnumerable<Round>> GetCompletedRoundsByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets rounds where a specific player was the judge.
    Task<IEnumerable<Round>> GetRoundsByJudgeIdAsync(Guid judgeId, CancellationToken cancellationToken = default);

    /// Gets rounds where a specific player was the winner.
    Task<IEnumerable<Round>> GetRoundsByWinnerIdAsync(Guid winnerId, CancellationToken cancellationToken = default);

    /// Adds a new round.
    Task AddAsync(Round round, CancellationToken cancellationToken = default);

    /// Updates an existing round.
    Task UpdateAsync(Round round, CancellationToken cancellationToken = default);

    /// Deletes a round.
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets the count of rounds in a game.
    Task<int> GetRoundCountByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets the count of completed rounds in a game.
    Task<int> GetCompletedRoundCountByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Saves all changes to the database.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}