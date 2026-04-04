using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class StartGameCommandTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid OwnerId = Guid.NewGuid();

    private static GameState BuildState(GameStateType stateType = GameStateType.Initial)
    {
        var state = GameState.Create(OwnerId, "Owner");
        state.GameId = GameId;
        state.State = stateType;
        return state;
    }

    private static StartGameCommand BuildCommand() => new();

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnStartGameCommand_EmitsGameStartedAndTransitionsToPlayerTurnStarting()
    {
        // Arrange
        var state = BuildState();

        // Act
        var result = new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        var started = Assert.IsType<GameStartedEvent>(Assert.Single(result.Events));
        Assert.Equal(GameId, started.GameId);
        Assert.Equal(OwnerId, started.ActivePlayerId);
        Assert.Equal(1, started.Round);

        Assert.Equal(GameStateType.PlayerTurnStarting, state.State);
        Assert.NotNull(result.OnEnterCommand);
    }

    [Fact]
    public void GameStateMachine_OnStartGameCommand_InitialisesTurnMarkerToFirstPlayer()
    {
        // Arrange
        var state = BuildState();
        var secondPlayerId = Guid.NewGuid();
        state.Players.Add(Player.Create(secondPlayerId, 1, "Player 2"));

        // Act
        new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        Assert.NotNull(state.TurnMarker);
        Assert.Equal(OwnerId, state.TurnMarker.ActivePlayerId);
        Assert.Equal(0, state.TurnMarker.ActivePlayerIndex);
        Assert.Equal(GameConfig.ActionsPerTurn, state.TurnMarker.ActionsRemaining);
    }

    [Fact]
    public void GameStateMachine_OnStartGameCommand_SetsRoundToOne()
    {
        // Arrange
        var state = BuildState();

        // Act
        new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        Assert.Equal(1, state.Round);
    }

    // ── Rejection scenarios ───────────────────────────────────────────────────

    [Theory]
    [InlineData(GameStateType.PlayerTurn)]
    [InlineData(GameStateType.VillainTurn)]
    [InlineData(GameStateType.Final)]
    public void GameStateMachine_OnStartGameCommand_WhenGameAlreadyStarted_Rejects(GameStateType stateType)
    {
        // Arrange
        var state = BuildState(stateType);

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand()).Events;

        // Assert
        Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
    }
}
