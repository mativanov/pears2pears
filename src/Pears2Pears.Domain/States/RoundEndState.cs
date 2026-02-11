using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.States;

/// State when a round has ended and results are displayed.
/// Brief pause before starting next round or ending game.
/// Transitions to: PlayingCardsState (next round) OR GameOverState (if winner reached target score).
public class RoundEndState : IGameState
{
    public GameStatus Status => GameStatus.InProgress;

    /// Players can join between rounds.
    public bool CanPlayerJoin() => true;

    /// Players can leave at any time.
    public bool CanPlayerLeave() => true;

    /// Game is already started.
    public bool CanStartGame() => false;

    /// Cannot play cards during round end display.
    public bool CanPlayCard() => false;

    /// Winner has already been selected.
    public bool CanSelectWinner() => false;

    /// Can start new round after brief display period.
    public bool CanStartNewRound() => true;

    public void OnEnter()
    {
        Console.WriteLine("Round ended. Displaying results...");
        // Could trigger a 3-second timer before auto-transitioning
    }

    public void OnExit()
    {
        Console.WriteLine("Starting next round or ending game...");
    }

    public IGameState? GetNextState()
    {
        // Transition logic will be handled by Game entity:
        // - If someone won: transition to GameOverState
        // - Otherwise: transition to PlayingCardsState (new round)
        return null;
    }
}