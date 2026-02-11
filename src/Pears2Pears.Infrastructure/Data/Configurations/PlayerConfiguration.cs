using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Data.Configurations;

/// Fluent API configuration for PlayerEntity.
public class PlayerConfiguration : IEntityTypeConfiguration<PlayerEntity>
{
    public void Configure(EntityTypeBuilder<PlayerEntity> builder)
    {
        // Table name
        builder.ToTable("Players");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Nickname)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Score)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.Role)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        builder.Property(p => p.IsConnected)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IsHost)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.JoinedAt)
            .IsRequired();

        builder.Property(p => p.LastActivityAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.GameId);

        builder.HasIndex(p => new { p.GameId, p.Nickname })
            .IsUnique(); // No duplicate nicknames in same game

        builder.HasIndex(p => p.IsHost);

        // Relationships

        // Many-to-one: Player -> Game (configured in GameConfiguration)

        // One-to-many: Player -> PlayerCards (hand)
        builder.HasMany(p => p.Hand)
            .WithOne(pc => pc.Player)
            .HasForeignKey(pc => pc.PlayerId)
            .OnDelete(DeleteBehavior.Cascade); // Delete hand when player is deleted

        // One-to-many: Player -> Rounds (as judge)
        builder.HasMany(p => p.RoundsAsJudge)
            .WithOne(r => r.Judge)
            .HasForeignKey(r => r.JudgeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if player was judge

        // One-to-many: Player -> Rounds (as winner)
        builder.HasMany(p => p.RoundsAsWinner)
            .WithOne(r => r.Winner)
            .HasForeignKey(r => r.WinnerId)
            .OnDelete(DeleteBehavior.SetNull); // Set to null if player is deleted
    }
}