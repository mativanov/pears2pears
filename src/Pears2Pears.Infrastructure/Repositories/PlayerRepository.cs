using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;
using Pears2Pears.Domain.Interfaces;
using Pears2Pears.Infrastructure.Data;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Repositories;

/// Repository implementation for Player entity.
public class PlayerRepository : IPlayerRepository
{
    private readonly GameDbContext _context;
    private readonly IMapper _mapper;

    public PlayerRepository(GameDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entity != null ? _mapper.Map<Player>(entity) : null;
    }

    public async Task<Player?> GetByIdWithHandAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Players
            .AsNoTracking()
            .Include(p => p.Hand)
                .ThenInclude(pc => pc.Card)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return entity != null ? _mapper.Map<Player>(entity) : null;
    }

    public async Task<IEnumerable<Player>> GetByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Players
            .AsNoTracking()
            .Where(p => p.GameId == gameId)
            .OrderBy(p => p.JoinedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Player>>(entities);
    }

    public async Task<IEnumerable<Player>> GetByGameIdAndRoleAsync(Guid gameId, PlayerRole role, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Players
            .AsNoTracking()
            .Where(p => p.GameId == gameId && p.Role == role)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Player>>(entities);
    }

    public async Task<Player?> GetJudgeByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.GameId == gameId && p.Role == PlayerRole.Judge, cancellationToken);

        return entity != null ? _mapper.Map<Player>(entity) : null;
    }

    public async Task<Player?> GetHostByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.GameId == gameId && p.IsHost, cancellationToken);

        return entity != null ? _mapper.Map<Player>(entity) : null;
    }

    public async Task<IEnumerable<Player>> GetConnectedPlayersByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Players
            .AsNoTracking()
            .Where(p => p.GameId == gameId && p.IsConnected)
            .OrderBy(p => p.JoinedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Player>>(entities);
    }

    public async Task<IEnumerable<Player>> GetLeaderboardByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Players
            .AsNoTracking()
            .Where(p => p.GameId == gameId)
            .OrderByDescending(p => p.Score)
            .ThenBy(p => p.Nickname)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Player>>(entities);
    }

    public async Task AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<PlayerEntity>(player);
        await _context.Players.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<PlayerEntity>(player);
        _context.Players.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Players.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.Players.Remove(entity);
        }
    }

    public async Task<bool> ExistsByNicknameInGameAsync(Guid gameId, string nickname, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .AnyAsync(p => p.GameId == gameId && p.Nickname.ToLower() == nickname.ToLower(), cancellationToken);
    }

    public async Task<int> GetPlayerCountByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return await _context.Players
            .CountAsync(p => p.GameId == gameId, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}