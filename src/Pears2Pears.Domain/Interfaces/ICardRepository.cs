using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.Interfaces;

/// Repository interface for Card entities (RedCard and GreenCard).
/// Defines contract for data access operations on cards.
public interface ICardRepository
{
    /// Gets a card by ID.
    Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets all cards of a specific type.
    Task<IEnumerable<Card>> GetByTypeAsync(CardType type, CancellationToken cancellationToken = default);

    /// Gets all red cards.
    Task<IEnumerable<RedCard>> GetAllRedCardsAsync(CancellationToken cancellationToken = default);

    /// Gets all green cards.
    Task<IEnumerable<GreenCard>> GetAllGreenCardsAsync(CancellationToken cancellationToken = default);

    /// Gets a random set of red cards.
    Task<IEnumerable<RedCard>> GetRandomRedCardsAsync(int count, CancellationToken cancellationToken = default);

    /// Gets a random set of green cards.
    Task<IEnumerable<GreenCard>> GetRandomGreenCardsAsync(int count, CancellationToken cancellationToken = default);

    /// Searches cards by text.
    Task<IEnumerable<Card>> SearchByTextAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// Gets cards by their IDs.
    Task<IEnumerable<Card>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// Adds a new card.
    Task AddAsync(Card card, CancellationToken cancellationToken = default);

    /// Adds multiple cards.
    Task AddRangeAsync(IEnumerable<Card> cards, CancellationToken cancellationToken = default);

    /// Updates an existing card.
    Task UpdateAsync(Card card, CancellationToken cancellationToken = default);

    /// Deletes a card.
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// Gets the total count of cards by type.
    Task<int> GetCountByTypeAsync(CardType type, CancellationToken cancellationToken = default);

    /// Saves all changes to the database.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}