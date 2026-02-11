using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.States;

/// State when game is created but waiting for minimum 4 players to join.
/// Transitions to: PlayingCardsState when game starts with 4+ players.
public class WaitingForPlayersState : IGameState
{
    public GameStatus Status => GameStatus.WaitingForPlayers;

    /// Players can join while waiting.
    public bool CanPlayerJoin() => true;

    /// Players can leave while waiting.
    public bool CanPlayerLeave() => true;

    /// Game can be started if minimum players are present.
    /// This check will be done in Game entity based on player count.
    public bool CanStartGame() => true;

    /// Cannot play cards while waiting for players.
    public bool CanPlayCard() => false;

    /// Cannot select winner while waiting for players.
    public bool CanSelectWinner() => false;

    /// Cannot start new round while waiting for players.
    public bool CanStartNewRound() => false;

    public void OnEnter()
    {
        // Log or trigger events when entering this state
        Console.WriteLine("Game is now waiting for players to join...");
    }

    public void OnExit()
    {
        // Log or trigger events when exiting this state
        Console.WriteLine("Minimum players reached. Starting game...");
    }

    public IGameState? GetNextState()
    {
        // Transition logic will be handled by Game entity
        // when StartGame() is called with sufficient players
        return null;
    }
}