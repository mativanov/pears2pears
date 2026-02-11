using Pears2Pears.Domain.Enums;

namespace Pears2Pears.Domain.States;

/// Interface for Game State Pattern.
/// Each state defines what actions are allowed and handles state transitions.
public interface IGameState
{
    /// The status this state represents.
    GameStatus Status { get; }

    /// Checks if a player can join the game in this state.
    bool CanPlayerJoin();

    /// Checks if a player can leave the game in this state.
    bool CanPlayerLeave();

    /// Checks if the game can start in this state.
    bool CanStartGame();

    /// Checks if cards can be played in this state.
    bool CanPlayCard();

    /// Checks if the judge can select a winner in this state.
    bool CanSelectWinner();

    /// Checks if a new round can be started in this state.
    bool CanStartNewRound();

    /// Handles entering this state.
    /// Called when transitioning TO this state.
    void OnEnter();

    /// Handles exiting this state.
    /// Called when transitioning FROM this state.
    void OnExit();

    /// Returns the next state based on game conditions.
    /// Returns null if no transition should occur.
    IGameState? GetNextState();
}