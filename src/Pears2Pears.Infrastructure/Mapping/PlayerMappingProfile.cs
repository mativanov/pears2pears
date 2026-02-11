using AutoMapper;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.ValueObjects;
using Pears2Pears.Infrastructure.Entities;

namespace Pears2Pears.Infrastructure.Mapping;

/// AutoMapper profile for Player entity mappings.
/// Maps between domain Player and PlayerEntity.
public class PlayerMappingProfile : Profile
{
    public PlayerMappingProfile()
    {
        // PlayerEntity -> Player
        CreateMap<PlayerEntity, Player>()
            .ConstructUsing(src => new Player(src.Nickname, src.GameId, src.IsHost))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Set in constructor
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => Score.From(src.Score)))
            .ForMember(dest => dest.Hand, opt => opt.MapFrom(src => MapHand(src.Hand)))
            .AfterMap((src, dest) =>
            {
                // Set role after construction
                dest.SetRole(src.Role);
                
                // Set connection state
                if (!src.IsConnected)
                    dest.Disconnect();
            });

        // Player -> PlayerEntity
        CreateMap<Player, PlayerEntity>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score.Value))
            .ForMember(dest => dest.Hand, opt => opt.Ignore()) // Handled separately
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.RoundsAsJudge, opt => opt.Ignore())
            .ForMember(dest => dest.RoundsAsWinner, opt => opt.Ignore());
    }

    /// Maps PlayerCardEntity collection to Hand domain object.
    private static Hand MapHand(ICollection<PlayerCardEntity> playerCards)
    {
        var hand = new Hand();
        
        var orderedCards = playerCards
            .OrderBy(pc => pc.OrderInHand)
            .Select(pc => new RedCard(pc.Card.Text, pc.Card.AdditionalInfo ?? string.Empty))
            .ToList();

        if (orderedCards.Any())
        {
            hand.AddCards(orderedCards);
        }

        return hand;
    }
}