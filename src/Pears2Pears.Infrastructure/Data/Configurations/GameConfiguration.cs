using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Data.Configurations;

/// Fluent API configuration for GameEntity.
public class GameConfiguration : IEntityTypeConfiguration<GameEntity>
{
    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        // Table name
        builder.ToTable("Games");

        // Primary key
        builder.HasKey(g => g.Id);

        // Properties
        builder.Property(g => g.Code)
            .IsRequired()
            .HasMaxLength(6)
            .IsFixedLength();

        builder.Property(g => g.Status)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string in database

        builder.Property(g => g.WinningScore)
            .IsRequired()
            .HasDefaultValue(7);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.RedCardDeckJson)
            .IsRequired()
            .HasDefaultValue("[]")
            .HasColumnType("jsonb"); // PostgreSQL JSON type

        builder.Property(g => g.GreenCardDeckJson)
            .IsRequired()
            .HasDefaultValue("[]")
            .HasColumnType("jsonb"); // PostgreSQL JSON type

        // Indexes
        builder.HasIndex(g => g.Code)
            .IsUnique();

        builder.HasIndex(g => g.Status);

        builder.HasIndex(g => g.CreatedAt);

        // Relationships

        // One-to-many: Game -> Players
        builder.HasMany(g => g.Players)
            .WithOne(p => p.Game)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Cascade); // Delete players when game is deleted

        // One-to-many: Game -> Rounds
        builder.HasMany(g => g.Rounds)
            .WithOne(r => r.Game)
            .HasForeignKey(r => r.GameId)
            .OnDelete(DeleteBehavior.Cascade); // Delete rounds when game is deleted

        // One-to-one (optional): Game -> CurrentRound
        builder.HasOne(g => g.CurrentRound)
            .WithMany()
            .HasForeignKey(g => g.CurrentRoundId)
            .OnDelete(DeleteBehavior.SetNull); // Set to null if round is deleted
    }
}