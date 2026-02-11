using AutoMapper;
using Pears2Pears.Domain.Entities;
using Pears2Pears.Domain.ValueObjects;
using Pears2Pears.Infrastructure.Entities;
using System.Text.Json;

namespace Pears2Pears.Infrastructure.Mapping;

/// AutoMapper profile for Game aggregate root mappings.
/// Maps between domain Game and GameEntity.
public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        // GameEntity -> Game
        CreateMap<GameEntity, Game>()
            .ConstructUsing(src => CreateGameFromEntity(src))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Set in constructor
            .ForMember(dest => dest.Code, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                // Initialize decks from JSON
                if (!string.IsNullOrWhiteSpace(src.RedCardDeckJson) && src.RedCardDeckJson != "[]")
                {
                    var redCardIds = JsonSerializer.Deserialize<List<Guid>>(src.RedCardDeckJson);
                }

                if (!string.IsNullOrWhiteSpace(src.GreenCardDeckJson) && src.GreenCardDeckJson != "[]")
                {
                    var greenCardIds = JsonSerializer.Deserialize<List<Guid>>(src.GreenCardDeckJson);
                }

                // Map players if included
                if (src.Players != null && src.Players.Any())
                {
                    foreach (var playerEntity in src.Players)
                    {
                        // Players are added through Game.AddPlayer() method
                        // This is handled at the service layer
                    }
                }
            });

        // Game -> GameEntity
        CreateMap<Game, GameEntity>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.Value))
            .ForMember(dest => dest.RedCardDeckJson, opt => opt.MapFrom(src => SerializeCardDeck(src)))
            .ForMember(dest => dest.GreenCardDeckJson, opt => opt.MapFrom(src => SerializeCardDeck(src)))
            .ForMember(dest => dest.CurrentRoundId, opt => opt.MapFrom(src => src.CurrentRound != null ? src.CurrentRound.Id : (Guid?)null))
            .ForMember(dest => dest.Players, opt => opt.Ignore()) // Handled separately via repository
            .ForMember(dest => dest.Rounds, opt => opt.Ignore()) // Handled separately via repository
            .ForMember(dest => dest.CurrentRound, opt => opt.Ignore());
    }

    /// Factory method to create Game from GameEntity.
    private static Game CreateGameFromEntity(GameEntity entity)
    {
        // Get the first player (host) if exists
        var hostNickname = entity.Players?.FirstOrDefault(p => p.IsHost)?.Nickname ?? "Unknown";
        
        var game = new Game(hostNickname, entity.WinningScore);
        
        // Use reflection to set the Id and Code (they are private set)
        var idProperty = typeof(Game).GetProperty(nameof(Game.Id));
        idProperty?.SetValue(game, entity.Id);

        var codeProperty = typeof(Game).GetProperty(nameof(Game.Code));
        codeProperty?.SetValue(game, GameCode.From(entity.Code));

        return game;
    }

    /// Serializes card deck to JSON (placeholder - returns empty array for now).
    private static string SerializeCardDeck(Game game)
    {
        return "[]";
    }
}