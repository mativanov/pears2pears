using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;
using Pears2Pears.Domain.Interfaces;
using Pears2Pears.Domain.ValueObjects;
using Pears2Pears.Infrastructure.Data;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Repositories;

/// Repository implementation for Game aggregate root.
/// Uses EF Core and AutoMapper to handle persistence.
public class GameRepository : IGameRepository
{
    private readonly GameDbContext _context;
    private readonly IMapper _mapper;

    public GameRepository(GameDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        return entity != null ? _mapper.Map<Game>(entity) : null;
    }

    public async Task<Game?> GetByCodeAsync(GameCode code, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Code == code.Value, cancellationToken);

        return entity != null ? _mapper.Map<Game>(entity) : null;
    }

    public async Task<Game?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Games
            .AsNoTracking()
            .Include(g => g.Players)
                .ThenInclude(p => p.Hand)
                    .ThenInclude(pc => pc.Card)
            .Include(g => g.Rounds)
                .ThenInclude(r => r.GreenCard)
            .Include(g => g.Rounds)
                .ThenInclude(r => r.Judge)
            .Include(g => g.Rounds)
                .ThenInclude(r => r.Winner)
            .Include(g => g.CurrentRound)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        return entity != null ? _mapper.Map<Game>(entity) : null;
    }

    public async Task<IEnumerable<Game>> GetActiveGamesAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Games
            .AsNoTracking()
            .Where(g => g.Status == GameStatus.WaitingForPlayers || g.Status == GameStatus.InProgress)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Game>>(entities);
    }

    public async Task<IEnumerable<Game>> GetGamesByStatusAsync(GameStatus status, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Games
            .AsNoTracking()
            .Where(g => g.Status == status)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Game>>(entities);
    }

    public async Task<IEnumerable<Game>> GetGamesByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Games
            .AsNoTracking()
            .Where(g => g.CreatedAt >= from && g.CreatedAt <= to)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Game>>(entities);
    }

    public async Task AddAsync(Game game, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<GameEntity>(game);
        await _context.Games.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<GameEntity>(game);
        _context.Games.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Games.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.Games.Remove(entity);
        }
    }

    public async Task<bool> ExistsByCodeAsync(GameCode code, CancellationToken cancellationToken = default)
    {
        return await _context.Games
            .AnyAsync(g => g.Code == code.Value, cancellationToken);
    }

    public async Task<int> GetActiveGameCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Games
            .CountAsync(g => g.Status == GameStatus.WaitingForPlayers || g.Status == GameStatus.InProgress, 
                cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}