using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.States;

/// State when players (non-judges) are playing their red cards to match the green card.
/// Transitions to: JudgingState when all non-judge players have played their cards.
public class PlayingCardsState : IGameState
{
    public GameStatus Status => GameStatus.InProgress;

    /// Players can still join during card playing (they'll join next round).
    public bool CanPlayerJoin() => true;

    /// Players can leave at any time.
    public bool CanPlayerLeave() => true;

    /// Game is already started, cannot start again.
    public bool CanStartGame() => false;

    /// This is the state where cards are played.
    public bool CanPlayCard() => true;

    /// Cannot select winner until all cards are played.
    public bool CanSelectWinner() => false;

    /// Cannot start new round while current round is in progress.
    public bool CanStartNewRound() => false;

    public void OnEnter()
    {
        Console.WriteLine("Round started. Players can now play their cards...");
    }

    public void OnExit()
    {
        Console.WriteLine("All cards played. Waiting for judge's decision...");
    }

    public IGameState? GetNextState()
    {
        // Transition to JudgingState will be triggered by Game entity
        // when all non-judge players have played their cards
        return null;
    }
}