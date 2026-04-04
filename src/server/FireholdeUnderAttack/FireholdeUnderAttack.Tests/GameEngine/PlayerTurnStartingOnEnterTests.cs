using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class PlayerTurnStartingOnEnterTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid PlayerId = Guid.NewGuid();

    private static GameState BuildState()
    {
        var state = GameState.Create(PlayerId, "Player 1");
        state.GameId = GameId;
        state.State = GameStateType.PlayerTurnStarting;
        state.Round = 2;
        state.TurnMarker = new TurnMarker
        {
            ActivePlayerId = PlayerId,
            ActivePlayerIndex = 0,
            ActionsRemaining = 3
        };
        return state;
    }

    [Fact]
    public void GameStateMachine_OnEnterPlayerTurnStarting_EmitsTurnChangedAndTransitionsToPlayerTurn()
    {
        // Arrange
        var state = BuildState();

        // Act
        var events = new GameStateMachine(state).Handle(new OnEnterCommand()).Events;

        // Assert
        var turnChanged = Assert.IsType<TurnChangedEvent>(Assert.Single(events));
        Assert.Equal(PlayerId, turnChanged.ActivePlayerId);
        Assert.Equal(GameId, turnChanged.GameId);
        Assert.Equal(2, turnChanged.Round);
        Assert.False(turnChanged.IsVillainTurn);
        Assert.Equal(GameStateType.PlayerTurn, state.State);
    }
}
