using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Data.Configurations;

/// Fluent API configuration for PlayerCardEntity (join table).
public class PlayerCardConfiguration : IEntityTypeConfiguration<PlayerCardEntity>
{
    public void Configure(EntityTypeBuilder<PlayerCardEntity> builder)
    {
        // Table name
        builder.ToTable("PlayerCards");

        // Composite primary key
        builder.HasKey(pc => new { pc.PlayerId, pc.CardId });

        // Properties
        builder.Property(pc => pc.AddedAt)
            .IsRequired();

        builder.Property(pc => pc.OrderInHand)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(pc => pc.PlayerId);

        builder.HasIndex(pc => pc.CardId);

        builder.HasIndex(pc => new { pc.PlayerId, pc.OrderInHand });

        // Relationships

        // Many-to-one: PlayerCard -> Player
        // Configured in PlayerConfiguration

        // Many-to-one: PlayerCard -> Card
        // Configured in CardConfiguration
    }
}