namespace Pears2Pears.Domain.Enums;

/// Represents the different states a game can be in during its lifecycle.
/// Used by the State Pattern to manage game flow.
public enum GameStatus
{
    WaitingForPlayers = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}