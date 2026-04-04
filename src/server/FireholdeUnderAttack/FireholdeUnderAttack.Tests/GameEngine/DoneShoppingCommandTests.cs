using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class DoneShoppingCommandTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid PlayerId = Guid.NewGuid();

    private static GameState BuildState(Guid? playerId = null)
    {
        var state = GameState.Create(playerId ?? PlayerId, "Player 1");
        state.GameId = GameId;
        state.State = GameStateType.Shopping;
        state.TurnMarker = new TurnMarker
        {
            ActivePlayerId = playerId ?? PlayerId,
            ActivePlayerIndex = 0,
            ActionsRemaining = 3
        };
        state.ShopInventory = ["healing_potion", "shrimp"];
        return state;
    }

    private static DoneShoppingCommand BuildCommand(Guid? playerId = null) =>
        new() { PlayerId = playerId ?? PlayerId };

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnDoneShoppingCommand_ClearsShopInventoryAndTransitionsToPlayerActionEnding()
    {
        // Arrange
        var state = BuildState();

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand()).Events;

        // Assert
        Assert.Empty(events);
        Assert.Empty(state.ShopInventory);
        Assert.Equal(GameStateType.PlayerActionEnding, state.State);
    }

    // ── Rejection scenarios ───────────────────────────────────────────────────

    [Theory]
    [InlineData(GameStateType.PlayerTurn)]
    [InlineData(GameStateType.Initial)]
    [InlineData(GameStateType.VillainTurn)]
    public void GameStateMachine_OnDoneShoppingCommand_WhenWrongGameState_Rejects(GameStateType stateType)
    {
        // Arrange
        var state = BuildState();
        state.State = stateType;

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand()).Events;

        // Assert
        Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
    }

    [Fact]
    public void GameStateMachine_OnDoneShoppingCommand_WhenNotActivePlayer_Rejects()
    {
        // Arrange
        var otherPlayerId = Guid.NewGuid();
        var state = BuildState();
        state.Players.Add(Player.Create(otherPlayerId, 1, "Player 2"));

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand(playerId: otherPlayerId)).Events;

        // Assert
        Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
    }
}
