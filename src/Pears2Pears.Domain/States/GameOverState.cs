using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.States;

/// Terminal state when game has ended and a winner is determined.
/// No further transitions from this state.
public class GameOverState : IGameState
{
    public GameStatus Status => GameStatus.Completed;

    /// Cannot join a completed game.
    public bool CanPlayerJoin() => false;

    /// Players can view results and leave.
    public bool CanPlayerLeave() => true;

    /// Game has already ended.
    public bool CanStartGame() => false;

    /// Cannot play cards in a finished game.
    public bool CanPlayCard() => false;

    /// Cannot select winner in a finished game.
    public bool CanSelectWinner() => false;

    /// Cannot start new round in a finished game.
    public bool CanStartNewRound() => false;

    public void OnEnter()
    {
        Console.WriteLine("Game Over! Displaying final results...");
    }

    public void OnExit()
    {
        // Terminal state - should not exit
        Console.WriteLine("Game ended. No further transitions.");
    }

    public IGameState? GetNextState()
    {
        // Terminal state - no transitions
        return null;
    }
}