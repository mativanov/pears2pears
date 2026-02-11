using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Data.Configurations;

/// Fluent API configuration for CardEntity.
public class CardConfiguration : IEntityTypeConfiguration<CardEntity>
{
    public void Configure(EntityTypeBuilder<CardEntity> builder)
    {
        // Table name
        builder.ToTable("Cards");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        builder.Property(c => c.AdditionalInfo)
            .HasMaxLength(200);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.Type);

        builder.HasIndex(c => c.Text); // For searching cards

        // Relationships

        // One-to-many: Card -> PlayerCards
        builder.HasMany(c => c.PlayerCards)
            .WithOne(pc => pc.Card)
            .HasForeignKey(pc => pc.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-many: Card -> Rounds (as green card)
        builder.HasMany(c => c.RoundsAsGreenCard)
            .WithOne(r => r.GreenCard)
            .HasForeignKey(r => r.GreenCardId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of cards used in rounds
    }
}