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
        return state;
    }

    private static MoveCommand BuildCommand(Guid? playerId = null) => new()
    {
        GameId = GameId,
        PlayerId = playerId ?? PlayerId
    };

    // ── State guard ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(GameStateType.Initial)]
    [InlineData(GameStateType.WaitingInLobby)]
    [InlineData(GameStateType.Final)]
    public void WhenNotInPlayingState_RejectsCommand(GameStateType stateType)
    {
        var state = BuildState(stateType);
        var events = new GameStateMachine(state).Handle(BuildCommand());

        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }

    // ── Validation ────────────────────────────────────────────────────────────

    [Fact]
    public void WhenPlayerNotInGame_RejectsCommand()
    {
        var state = BuildState(GameStateType.Playing);
        var command = BuildCommand(playerId: Guid.NewGuid()); // unknown player

        var events = new GameStateMachine(state).Handle(command);

        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }

    [Fact]
    public void WhenRejected_EventCarriesGameId()
    {
        var state = BuildState(GameStateType.Playing);
        var command = BuildCommand(playerId: Guid.NewGuid());

        var events = new GameStateMachine(state).Handle(command);

        var rejected = Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
        Assert.Equal(GameId, rejected.GameId);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public void WhenValid_EmitsSingleMoveEvent()
    {
        var state = BuildState(GameStateType.Playing);

        var events = new GameStateMachine(state).Handle(BuildCommand());

        var move = Assert.IsType<MoveEvent>(Assert.Single(events));
        Assert.Equal(PlayerId, move.PlayerId);
    }

    [Fact]
    public void WhenValid_DiceRollIsInRange()
    {
        var state = BuildState(GameStateType.Playing);

        var move = Assert.IsType<MoveEvent>(
            Assert.Single(new GameStateMachine(state).Handle(BuildCommand())));

        Assert.InRange(move.DiceRoll, 1, 6);
    }

    [Fact]
    public void WhenValid_PlayerTileIsUpdated()
    {
        var state = BuildState(GameStateType.Playing);
        var initialTile = state.Players.First(p => p.Id == PlayerId).CurrentTile;

        var move = Assert.IsType<MoveEvent>(
            Assert.Single(new GameStateMachine(state).Handle(BuildCommand())));

        Assert.True(move.NewTileId > initialTile);
        Assert.Equal(move.NewTileId, state.Players.First(p => p.Id == PlayerId).CurrentTile);
    }
}
