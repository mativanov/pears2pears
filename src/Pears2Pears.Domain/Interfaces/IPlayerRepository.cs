using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.Interfaces;

/// Repository interface for Player entity.
/// Defines contract for data access operations on players.
public interface IPlayerRepository
{
    /// Gets a player by ID.
    Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets a player with their hand of cards.
    Task<Player?> GetByIdWithHandAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets all players in a specific game.
    Task<IEnumerable<Player>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets players by role in a specific game.
    Task<IEnumerable<Player>> GetByGameIdAndRoleAsync(Guid gameId, PlayerRole role, CancellationToken cancellationToken = default);

    /// Gets the current judge in a game.
    Task<Player?> GetJudgeByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets the host player of a game.
    Task<Player?> GetHostByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets connected players in a game.
    Task<IEnumerable<Player>> GetConnectedPlayersByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Gets top players by score in a game.
    Task<IEnumerable<Player>> GetLeaderboardByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Adds a new player.
    Task AddAsync(Player player, CancellationToken cancellationToken = default);

    /// Updates an existing player.
    Task UpdateAsync(Player player, CancellationToken cancellationToken = default);

    /// Deletes a player.
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// Checks if a nickname exists in a specific game.
    Task<bool> ExistsByNicknameInGameAsync(Guid gameId, string nickname, CancellationToken cancellationToken = default);

    /// Gets the count of players in a game.
    Task<int> GetPlayerCountByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// Saves all changes to the database.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}