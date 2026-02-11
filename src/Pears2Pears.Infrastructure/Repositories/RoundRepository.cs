using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Interfaces;
using Pears2Pears.Infrastructure.Data;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Repositories;

/// Repository implementation for Round entity.
public class RoundRepository : IRoundRepository
{
    private readonly GameDbContext _context;
    private readonly IMapper _mapper;

    public RoundRepository(GameDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Round?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Rounds
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return entity != null ? _mapper.Map<Round>(entity) : null;
    }

    public async Task<Round?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Rounds
            .AsNoTracking()
            .Include(r => r.GreenCard)
            .Include(r => r.Judge)
            .Include(r => r.Winner)
            .Include(r => r.WinningCard)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return entity != null ? _mapper.Map<Round>(entity) : null;
    }

    public async Task<IEnumerable<Round>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Rounds
            .AsNoTracking()
            .Where(r => r.GameId == gameId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Round>>(entities);
    }

    public async Task<IEnumerable<Round>> GetByGameIdOrderedAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Rounds
            .AsNoTracking()
            .Where(r => r.GameId == gameId)
            .OrderBy(r => r.RoundNumber)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Round>>(entities);
    }

    public async Task<Round?> GetCurrentRoundByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Rounds
            .AsNoTracking()
            .Include(r => r.GreenCard)
            .Include(r => r.Judge)
            .Where(r => r.GameId == gameId)
            .OrderByDescending(r => r.RoundNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return entity != null ? _mapper.Map<Round>(entity) : null;
    }

    public async Task<Round?> GetByGameIdAndRoundNumberAsync(Guid gameId, int roundNumber, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Rounds
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.GameId == gameId && r.RoundNumber == roundNumber, cancellationToken);

        return entity != null ? _mapper.Map<Round>(entity) : null;
    }

    public async Task<IEnumerable<Round>> GetCompletedRoundsByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Rounds
            .AsNoTracking()
            .Where(r => r.GameId == gameId && r.WinnerId != null && r.EndedAt != null)
            .OrderBy(r => r.RoundNumber)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Round>>(entities);
    }

    public async Task<IEnumerable<Round>> GetRoundsByJudgeIdAsync(Guid judgeId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Rounds
            .AsNoTracking()
            .Where(r => r.JudgeId == judgeId)
            .OrderBy(r => r.StartedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Round>>(entities);
    }

    public async Task<IEnumerable<Round>> GetRoundsByWinnerIdAsync(Guid winnerId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Rounds
            .AsNoTracking()
            .Where(r => r.WinnerId == winnerId)
            .OrderBy(r => r.StartedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Round>>(entities);
    }

    public async Task AddAsync(Round round, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<RoundEntity>(round);
        await _context.Rounds.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Round round, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<RoundEntity>(round);
        _context.Rounds.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Rounds.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.Rounds.Remove(entity);
        }
    }

    public async Task<int> GetRoundCountByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return await _context.Rounds
            .CountAsync(r => r.GameId == gameId, cancellationToken);
    }

    public async Task<int> GetCompletedRoundCountByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return await _context.Rounds
            .CountAsync(r => r.GameId == gameId && r.WinnerId != null && r.EndedAt != null, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}