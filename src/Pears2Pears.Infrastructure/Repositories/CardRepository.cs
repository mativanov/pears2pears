using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;
using Pears2Pears.Domain.Interfaces;
using Pears2Pears.Infrastructure.Data;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Repositories;

/// Repository implementation for Card entities.
public class CardRepository : ICardRepository
{
    private readonly GameDbContext _context;
    private readonly IMapper _mapper;

    public CardRepository(GameDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Cards
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return entity != null ? _mapper.Map<Card>(entity) : null;
    }

    public async Task<IEnumerable<Card>> GetByTypeAsync(CardType type, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Cards
            .AsNoTracking()
            .Where(c => c.Type == type)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Card>>(entities);
    }

    public async Task<IEnumerable<RedCard>> GetAllRedCardsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Cards
            .AsNoTracking()
            .Where(c => c.Type == CardType.Red)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<RedCard>>(entities);
    }

    public async Task<IEnumerable<GreenCard>> GetAllGreenCardsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Cards
            .AsNoTracking()
            .Where(c => c.Type == CardType.Green)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<GreenCard>>(entities);
    }

    public async Task<IEnumerable<RedCard>> GetRandomRedCardsAsync(int count, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Cards
            .AsNoTracking()
            .Where(c => c.Type == CardType.Red)
            .OrderBy(c => Guid.NewGuid()) // Random ordering
            .Take(count)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<RedCard>>(entities);
    }

    public async Task<IEnumerable<GreenCard>> GetRandomGreenCardsAsync(int count, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Cards
            .AsNoTracking()
            .Where(c => c.Type == CardType.Green)
            .OrderBy(c => Guid.NewGuid()) // Random ordering
            .Take(count)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<GreenCard>>(entities);
    }

    public async Task<IEnumerable<Card>> SearchByTextAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Cards
            .AsNoTracking()
            .Where(c => c.Text.Contains(searchTerm))
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Card>>(entities);
    }

    public async Task<IEnumerable<Card>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Cards
            .AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Card>>(entities);
    }

    public async Task AddAsync(Card card, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<CardEntity>(card);
        await _context.Cards.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Card> cards, CancellationToken cancellationToken = default)
    {
        var entities = _mapper.Map<IEnumerable<CardEntity>>(cards);
        await _context.Cards.AddRangeAsync(entities, cancellationToken);
    }

    public async Task UpdateAsync(Card card, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<CardEntity>(card);
        _context.Cards.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Cards.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.Cards.Remove(entity);
        }
    }

    public async Task<int> GetCountByTypeAsync(CardType type, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .CountAsync(c => c.Type == type, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}