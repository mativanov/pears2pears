using AutoMapper;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.Enums;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Mapping;

/// AutoMapper profile for Card entity mappings.
/// Maps between domain Card entities (RedCard, GreenCard) and CardEntity.
public class CardMappingProfile : Profile
{
    public CardMappingProfile()
    {
        // CardEntity -> Card (base mapping)
        CreateMap<CardEntity, Card>()
            .ConstructUsing(src => CreateCardFromEntity(src))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Set in constructor
            .ForMember(dest => dest.Text, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // CardEntity -> RedCard
        CreateMap<CardEntity, RedCard>()
            .IncludeBase<CardEntity, Card>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.AdditionalInfo));

        // CardEntity -> GreenCard
        CreateMap<CardEntity, GreenCard>()
            .IncludeBase<CardEntity, Card>()
            .ForMember(dest => dest.Synonyms, opt => opt.MapFrom(src => src.AdditionalInfo));

        // Card -> CardEntity (base mapping)
        CreateMap<Card, CardEntity>()
            .ForMember(dest => dest.AdditionalInfo, opt => opt.Ignore())
            .ForMember(dest => dest.PlayerCards, opt => opt.Ignore())
            .ForMember(dest => dest.RoundsAsGreenCard, opt => opt.Ignore());

        // RedCard -> CardEntity
        CreateMap<RedCard, CardEntity>()
            .IncludeBase<Card, CardEntity>()
            .ForMember(dest => dest.AdditionalInfo, opt => opt.MapFrom(src => src.Description));

        // GreenCard -> CardEntity
        CreateMap<GreenCard, CardEntity>()
            .IncludeBase<Card, CardEntity>()
            .ForMember(dest => dest.AdditionalInfo, opt => opt.MapFrom(src => src.Synonyms));
    }

    /// Factory method to create correct Card subtype based on CardType.
    private static Card CreateCardFromEntity(CardEntity entity)
    {
        return entity.Type switch
        {
            CardType.Red => new RedCard(entity.Text, entity.AdditionalInfo ?? string.Empty),
            CardType.Green => new GreenCard(entity.Text, entity.AdditionalInfo ?? string.Empty),
            _ => throw new ArgumentException($"Unknown card type: {entity.Type}")
        };
    }
}