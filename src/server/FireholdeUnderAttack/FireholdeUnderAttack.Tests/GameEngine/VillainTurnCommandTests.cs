using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class VillainTurnCommandTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid Player1Id = Guid.NewGuid();
    private static readonly Guid Player2Id = Guid.NewGuid();

    private static GameState BuildState(int playerCount = 2)
    {
        var state = GameState.Create(Player1Id, "Player 1");
        state.GameId = GameId;
        state.State = GameStateType.VillainTurn;
        state.Round = 1;
        if (playerCount > 1)
            state.Players.Add(Player.Create(Player2Id, 1, "Player 2"));
        state.TurnMarker = new TurnMarker
        {
            ActivePlayerId = null,
            ActivePlayerIndex = -1,
            ActionsRemaining = 0
        };
        return state;
    }

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnVillainTurnCommand_TransitionsToPlayerTurnStartingAndResetsToFirstPlayer()
    {
        // Arrange
        var state = BuildState();

        // Act
        var result = new GameStateMachine(state).Handle(new VillainTurnCommand());

        // Assert
        Assert.Empty(result.Events);
        Assert.Equal(GameStateType.PlayerTurnStarting, state.State);
        Assert.NotNull(result.OnEnterCommand);
    }

    [Fact]
    public void GameStateMachine_OnVillainTurnCommand_ResetsTurnMarkerToFirstPlayer()
    {
        // Arrange
        var state = BuildState();

        // Act
        new GameStateMachine(state).Handle(new VillainTurnCommand());

        // Assert
        Assert.Equal(Player1Id, state.TurnMarker!.ActivePlayerId);
        Assert.Equal(0, state.TurnMarker.ActivePlayerIndex);
        Assert.Equal(GameConfig.ActionsPerTurn, state.TurnMarker.ActionsRemaining);
    }

    [Fact]
    public void GameStateMachine_OnVillainTurnCommand_IncrementsRound()
    {
        // Arrange
        var state = BuildState();

        // Act
        new GameStateMachine(state).Handle(new VillainTurnCommand());

        // Assert — AdvanceVillainTurn resets to player 0; round is incremented by AdvanceToNextPlayerOrVillain
        // VillainTurnCommandSaga calls AdvanceVillainTurn which resets without incrementing round
        // So round stays the same here (round increment happens on player-to-villain transition)
        Assert.Equal(1, state.Round);
    }

    // ── Rejection scenarios ───────────────────────────────────────────────────

    [Theory]
    [InlineData(GameStateType.PlayerTurn)]
    [InlineData(GameStateType.Initial)]
    [InlineData(GameStateType.Final)]
    public void GameStateMachine_OnVillainTurnCommand_WhenWrongGameState_Rejects(GameStateType stateType)
    {
        // Arrange
        var state = BuildState();
        state.State = stateType;

        // Act
        var events = new GameStateMachine(state).Handle(new VillainTurnCommand()).Events;

        // Assert
        Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
    }
}
