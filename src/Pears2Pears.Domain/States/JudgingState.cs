using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.States;

/// State when all cards have been played and the judge is selecting the winner.
/// This implements pessimistic locking - only the judge has write access.
/// Transitions to: RoundEndState when judge selects a winner.
public class JudgingState : IGameState
{
    public GameStatus Status => GameStatus.InProgress;

    /// Players can join but won't participate in current round.
    public bool CanPlayerJoin() => true;

    /// Players can leave at any time.
    public bool CanPlayerLeave() => true;

    /// Game is already started.
    public bool CanStartGame() => false;

    /// Cannot play more cards during judging phase.
    /// All cards have already been submitted.
    public bool CanPlayCard() => false;

    /// This is the state where judge selects the winner.
    /// Implements FR6: Pessimistic locking - only judge can select.
    public bool CanSelectWinner() => true;

    /// Cannot start new round until current round is completed.
    public bool CanStartNewRound() => false;

    public void OnEnter()
    {
        Console.WriteLine("Judging phase. Only the judge can select the winning card...");
    }

    public void OnExit()
    {
        Console.WriteLine("Winner selected. Round completed...");
    }

    public IGameState? GetNextState()
    {
        // Transition to RoundEndState will be triggered by Game entity
        // when judge selects a winner
        return null;
    }
}