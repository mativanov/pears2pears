using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;
using Pears2Pears.Domain.ValueObjects;

namespace Pears2Pears.Domain.Interfaces;

/// Repository interface for Game aggregate root.
/// Defines contract for data access operations on games.
public interface IGameRepository
{
    /// Gets a game by its unique ID.
    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets a game by its unique code.
    Task<Game?> GetByCodeAsync(GameCode code, CancellationToken cancellationToken = default);

    /// Gets a game with all related data (players, rounds, cards).
    Task<Game?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets all active games (not completed or cancelled).
    Task<IEnumerable<Game>> GetActiveGamesAsync(CancellationToken cancellationToken = default);

    /// Gets games by status.
    Task<IEnumerable<Game>> GetGamesByStatusAsync(GameStatus status, CancellationToken cancellationToken = default);

    /// Gets games created within a time range.
    Task<IEnumerable<Game>> GetGamesByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// Adds a new game to the repository.
    Task AddAsync(Game game, CancellationToken cancellationToken = default);

    /// Updates an existing game.
    Task UpdateAsync(Game game, CancellationToken cancellationToken = default);

    /// Deletes a game.
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// Checks if a game with the given code exists.
    Task<bool> ExistsByCodeAsync(GameCode code, CancellationToken cancellationToken = default);

    /// Gets the count of active games.
    Task<int> GetActiveGameCountAsync(CancellationToken cancellationToken = default);

    /// Saves all changes to the database.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}