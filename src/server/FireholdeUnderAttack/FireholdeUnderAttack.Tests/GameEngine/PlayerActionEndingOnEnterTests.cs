using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class PlayerActionEndingOnEnterTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid Player1Id = Guid.NewGuid();
    private static readonly Guid Player2Id = Guid.NewGuid();

    private static GameState BuildState(int actionsRemaining, int playerCount = 1)
    {
        var state = GameState.Create(Player1Id, "Player 1");
        state.GameId = GameId;
        state.State = GameStateType.PlayerActionEnding;
        if (playerCount > 1)
            state.Players.Add(Player.Create(Player2Id, 1, "Player 2"));
        state.TurnMarker = new TurnMarker
        {
            ActivePlayerId = Player1Id,
            ActivePlayerIndex = 0,
            ActionsRemaining = actionsRemaining
        };
        return state;
    }

    [Fact]
    public void GameStateMachine_OnEnterPlayerActionEnding_WhenActionsRemain_DecrementsAndTransitionsToPlayerTurn()
    {
        // Arrange
        var state = BuildState(actionsRemaining: 2);

        // Act
        new GameStateMachine(state).Handle(new OnEnterCommand());

        // Assert
        Assert.Equal(GameStateType.PlayerTurn, state.State);
        Assert.Equal(1, state.TurnMarker!.ActionsRemaining);
    }

    [Fact]
    public void GameStateMachine_OnEnterPlayerActionEnding_WhenLastActionAndMorePlayers_TransitionsToPlayerTurnStarting()
    {
        // Arrange
        var state = BuildState(actionsRemaining: 1, playerCount: 2);

        // Act
        new GameStateMachine(state).Handle(new OnEnterCommand());

        // Assert
        Assert.Equal(GameStateType.PlayerTurnStarting, state.State);
        Assert.Equal(Player2Id, state.TurnMarker!.ActivePlayerId);
    }

    [Fact]
    public void GameStateMachine_OnEnterPlayerActionEnding_WhenLastActionAndLastPlayer_TransitionsToVillainTurn()
    {
        // Arrange
        var state = BuildState(actionsRemaining: 1, playerCount: 1);

        // Act
        new GameStateMachine(state).Handle(new OnEnterCommand());

        // Assert
        Assert.Equal(GameStateType.VillainTurn, state.State);
    }
}
