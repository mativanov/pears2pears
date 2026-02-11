using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Data.Configurations;

/// Fluent API configuration for RoundEntity.
public class RoundConfiguration : IEntityTypeConfiguration<RoundEntity>
{
    public void Configure(EntityTypeBuilder<RoundEntity> builder)
    {
        // Table name
        builder.ToTable("Rounds");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.RoundNumber)
            .IsRequired();

        builder.Property(r => r.AllCardsPlayed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.PlayedCardsJson)
            .IsRequired()
            .HasDefaultValue("{}")
            .HasColumnType("jsonb"); // PostgreSQL JSON type

        builder.Property(r => r.StartedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.GameId);

        builder.HasIndex(r => new { r.GameId, r.RoundNumber })
            .IsUnique(); // No duplicate round numbers in same game

        builder.HasIndex(r => r.JudgeId);

        builder.HasIndex(r => r.WinnerId);

        // Relationships

        // Many-to-one: Round -> Game (configured in GameConfiguration)

        // Many-to-one: Round -> Judge (Player)
        // Configured in PlayerConfiguration

        // Many-to-one: Round -> GreenCard
        builder.HasOne(r => r.GreenCard)
            .WithMany(c => c.RoundsAsGreenCard)
            .HasForeignKey(r => r.GreenCardId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-one: Round -> Winner (Player, optional)
        // Configured in PlayerConfiguration

        // Many-to-one: Round -> WinningCard (optional)
        builder.HasOne(r => r.WinningCard)
            .WithMany()
            .HasForeignKey(r => r.WinningCardId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}