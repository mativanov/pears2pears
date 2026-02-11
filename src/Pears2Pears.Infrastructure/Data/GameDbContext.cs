using Microsoft.EntityFrameworkCore;
using Pears2Pears.Infrastructure.Data.Configurations;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Data;

/// Entity Framework DbContext for the Pears2Pears game.
/// Manages all database operations and entity configurations.
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    // DbSets - represent tables in the database

    public DbSet<GameEntity> Games { get; set; } = null!;
    public DbSet<PlayerEntity> Players { get; set; } = null!;
    public DbSet<CardEntity> Cards { get; set; } = null!;
    public DbSet<RoundEntity> Rounds { get; set; } = null!;
    public DbSet<PlayerCardEntity> PlayerCards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Fluent API configurations
        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new CardConfiguration());
        modelBuilder.ApplyConfiguration(new RoundConfiguration());
        modelBuilder.ApplyConfiguration(new PlayerCardConfiguration());
    }

    /// Override SaveChanges to add automatic timestamp updates.
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// Override SaveChangesAsync to add automatic timestamp updates.
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// Automatically updates timestamps for entities.
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Handle different entity types
            if (entry.Entity is GameEntity game && entry.State == EntityState.Added)
            {
                game.CreatedAt = DateTime.UtcNow;
            }

            if (entry.Entity is PlayerEntity player)
            {
                if (entry.State == EntityState.Added)
                    player.JoinedAt = DateTime.UtcNow;
                
                player.LastActivityAt = DateTime.UtcNow;
            }

            if (entry.Entity is CardEntity card && entry.State == EntityState.Added)
            {
                card.CreatedAt = DateTime.UtcNow;
            }

            if (entry.Entity is RoundEntity round && entry.State == EntityState.Added)
            {
                round.StartedAt = DateTime.UtcNow;
            }

            if (entry.Entity is PlayerCardEntity playerCard && entry.State == EntityState.Added)
            {
                playerCard.AddedAt = DateTime.UtcNow;
            }
        }
    }
}