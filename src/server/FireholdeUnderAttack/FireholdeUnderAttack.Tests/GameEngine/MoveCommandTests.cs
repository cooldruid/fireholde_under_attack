using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class MoveCommandTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid PlayerId = Guid.NewGuid();

    private static GameState BuildState(GameStateType stateType, Guid? playerId = null)
    {
        var state = GameState.Create(playerId ?? PlayerId, "TestPlayer");
        state.GameId = GameId;
        state.State = stateType;
        if (stateType == GameStateType.PlayerTurn)
        {
            state.TurnMarker = new TurnMarker
            {
                ActivePlayerId = playerId ?? PlayerId,
                ActivePlayerIndex = 0,
                ActionsRemaining = 3
            };
        }
        return state;
    }

    private static MoveCommand BuildCommand(Guid? playerId = null) => new()
    {
        PlayerId = playerId ?? PlayerId
    };

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnMoveCommand_EmitsMoveEventAndTransitionsToPlayerTurnStarting()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        state.Board = Board.Create(Enumerable.Range(0, 36).Select(i => new Tile { Id = i, Type = BoardTileType.Empty }).ToList());
        var initialTile = state.Players.First(p => p.PlayerId == PlayerId).CurrentTile;

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand()).Events;

        // Assert
        var move = Assert.IsType<MoveEvent>(Assert.Single(events));
        Assert.Equal(PlayerId, move.PlayerId);
        Assert.InRange(move.DiceRoll, 1, 6);
        Assert.NotEqual(initialTile, move.NewTileId);
        Assert.Equal(move.NewTileId, state.Players.First(p => p.PlayerId == PlayerId).CurrentTile);

        Assert.Equal(GameStateType.PlayerTurnStarting, state.State);
    }

    [Fact]
    public void GameStateMachine_OnMoveCommand_DecrementsActionsRemaining()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);

        // Act
        new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        Assert.Equal(2, state.TurnMarker!.ActionsRemaining);
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
        var events = new GameStateMachine(state).Handle(BuildCommand()).Events;

        // Assert
        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }

    [Fact]
    public void GameStateMachine_OnMoveCommand_WhenPlayerNotInGame_RejectsWithGameId()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        var command = BuildCommand(playerId: Guid.NewGuid());

        // Act
        var events = new GameStateMachine(state).Handle(command).Events;

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
        state.Players.Add(Player.Create(secondPlayerId, 1, "Player 2"));
        var command = BuildCommand(playerId: secondPlayerId);

        // Act
        var events = new GameStateMachine(state).Handle(command).Events;

        // Assert
        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }
}
