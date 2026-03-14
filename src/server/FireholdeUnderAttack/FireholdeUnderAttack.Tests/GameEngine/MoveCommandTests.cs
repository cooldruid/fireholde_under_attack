using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class MoveCommandTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid PlayerId = Guid.NewGuid();

    private static GameState BuildState(GameStateType stateType, Guid? playerId = null)
    {
        var state = GameState.Create(playerId ?? PlayerId);
        state.State = stateType;
        if (stateType == GameStateType.PlayerTurn)
        {
            state.ActivePlayerId = playerId ?? PlayerId;
            state.ActivePlayerIndex = 0;
        }
        return state;
    }

    private static MoveCommand BuildCommand(Guid? playerId = null) => new()
    {
        GameId = GameId,
        PlayerId = playerId ?? PlayerId
    };

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnMoveCommand_MovesPlayerAndAdvancesToNextPlayer()
    {
        // Arrange
        var secondPlayerId = Guid.NewGuid();
        var state = BuildState(GameStateType.PlayerTurn);
        state.Players.Add(new PlayerState { Id = secondPlayerId, CurrentTile = 1, Health = 50 });
        var initialTile = state.Players.First(p => p.Id == PlayerId).CurrentTile;

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        Assert.Equal(2, events.Count);

        var move = Assert.IsType<MoveEvent>(events[0]);
        Assert.Equal(PlayerId, move.PlayerId);
        Assert.InRange(move.DiceRoll, 1, 6);
        Assert.True(move.NewTileId > initialTile);
        Assert.Equal(move.NewTileId, state.Players.First(p => p.Id == PlayerId).CurrentTile);

        var turnChanged = Assert.IsType<TurnChangedEvent>(events[1]);
        Assert.False(turnChanged.IsVillainTurn);
        Assert.Equal(secondPlayerId, turnChanged.ActivePlayerId);
        Assert.Equal(GameId, turnChanged.GameId);
        Assert.Equal(secondPlayerId, state.ActivePlayerId);
    }

    [Fact]
    public void GameStateMachine_OnMoveCommand_AfterLastPlayer_TransitionsToVillainTurn()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn); // single player = last player

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        Assert.Equal(2, events.Count);
        Assert.IsType<MoveEvent>(events[0]);

        var turnChanged = Assert.IsType<TurnChangedEvent>(events[1]);
        Assert.True(turnChanged.IsVillainTurn);
        Assert.Null(turnChanged.ActivePlayerId);
        Assert.Equal(GameId, turnChanged.GameId);

        Assert.Equal(GameStateType.VillainTurn, state.State);
        Assert.Null(state.ActivePlayerId);
        Assert.Equal(1, state.Round);
    }

    // ── Rejection scenarios ───────────────────────────────────────────────────

    [Theory]
    [InlineData(GameStateType.Initial)]
    [InlineData(GameStateType.VillainTurn)]
    [InlineData(GameStateType.Final)]
    public void GameStateMachine_OnMoveCommand_WhenWrongGameState_Rejects(GameStateType stateType)
    {
        // Arrange
        var state = BuildState(stateType);

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }

    [Fact]
    public void GameStateMachine_OnMoveCommand_WhenPlayerNotInGame_RejectsWithGameId()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        var command = BuildCommand(playerId: Guid.NewGuid()); // unknown player

        // Act
        var events = new GameStateMachine(state).Handle(command);

        // Assert
        var rejected = Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
        Assert.Equal(GameId, rejected.GameId);
    }

    [Fact]
    public void GameStateMachine_OnMoveCommand_WhenNotActivePlayer_Rejects()
    {
        // Arrange
        var secondPlayerId = Guid.NewGuid();
        var state = BuildState(GameStateType.PlayerTurn);
        state.Players.Add(new PlayerState { Id = secondPlayerId, CurrentTile = 1, Health = 50 });
        var command = BuildCommand(playerId: secondPlayerId); // valid player, but not active

        // Act
        var events = new GameStateMachine(state).Handle(command);

        // Assert
        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }
}
