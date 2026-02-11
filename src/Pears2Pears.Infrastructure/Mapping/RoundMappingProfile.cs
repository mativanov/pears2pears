using AutoMapper;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Infrastructure.Entities;
using System.Text.Json;

namespace Pears2Pears.Infrastructure.Mapping;

/// AutoMapper profile for Round entity mappings.
/// Maps between domain Round and RoundEntity.
public class RoundMappingProfile : Profile
{
    public RoundMappingProfile()
    {
        // RoundEntity -> Round
        CreateMap<RoundEntity, Round>()
            .ConstructUsing(src => new Round(
                src.GameId,
                src.RoundNumber,
                src.JudgeId,
                new GreenCard(src.GreenCard.Text, src.GreenCard.AdditionalInfo ?? string.Empty)
            ))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Set in constructor
            .AfterMap((src, dest) =>
            {
                // Deserialize played cards from JSON
                if (!string.IsNullOrWhiteSpace(src.PlayedCardsJson) && src.PlayedCardsJson != "{}")
                {
                    var playedCards = JsonSerializer.Deserialize<Dictionary<string, string>>(src.PlayedCardsJson);
                    
                    if (playedCards != null)
                    {
                        foreach (var kvp in playedCards)
                        {
                            var playerId = Guid.Parse(kvp.Key);
                            var cardId = Guid.Parse(kvp.Value);
                            
                            // Note: In real scenario, we'd need to fetch the actual RedCard
                            // For now, we'll create a placeholder
                            var card = new RedCard($"Card {cardId}");
                            dest.PlayCard(playerId, card);
                        }
                    }
                }

                // Mark all cards played if needed
                if (src.AllCardsPlayed)
                {
                    dest.MarkAllCardsPlayed();
                }

                // Select winner if exists
                if (src.WinnerId.HasValue && src.WinningCardId.HasValue)
                {
                    // Winner selection is handled in Game aggregate
                    // Here we just acknowledge it exists
                }
            });

        // Round -> RoundEntity
        CreateMap<Round, RoundEntity>()
            .ForMember(dest => dest.GreenCardId, opt => opt.MapFrom(src => src.GreenCard.Id))
            .ForMember(dest => dest.PlayedCardsJson, opt => opt.MapFrom(src => SerializePlayedCards(src)))
            .ForMember(dest => dest.WinningCardId, opt => opt.MapFrom(src => src.WinningCard != null ? src.WinningCard.Id : (Guid?)null))
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.GreenCard, opt => opt.Ignore())
            .ForMember(dest => dest.Judge, opt => opt.Ignore())
            .ForMember(dest => dest.Winner, opt => opt.Ignore())
            .ForMember(dest => dest.WinningCard, opt => opt.Ignore());
    }

    /// Serializes played cards dictionary to JSON string.
    private static string SerializePlayedCards(Round round)
    {
        var playedCards = round.PlayedCards
            .ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value.Id.ToString()
            );

        return JsonSerializer.Serialize(playedCards);
    }
}