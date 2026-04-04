using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class BuyCardCommandTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid PlayerId = Guid.NewGuid();
    private const string CardId = "healing_potion"; // level 1, price 20

    private static GameState BuildState(Guid? playerId = null, int gold = 100)
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
        state.Players.First().Gold = gold;
        state.ShopInventory = [CardId, "shrimp"];
        return state;
    }

    private static BuyCardCommand BuildCommand(Guid? playerId = null, string cardId = CardId) =>
        new() { PlayerId = playerId ?? PlayerId, CardId = cardId };

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnBuyCardCommand_DeductsGoldAddsToInventoryEmitsCardAcquired()
    {
        // Arrange
        var state = BuildState(gold: 100);

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand()).Events;

        // Assert
        var acquired = Assert.IsType<CardAcquiredEvent>(Assert.Single(events));
        Assert.Equal(PlayerId, acquired.PlayerId);
        Assert.Equal(CardId, acquired.CardId);
        Assert.Equal(GameId, acquired.GameId);

        var player = state.Players.First(p => p.PlayerId == PlayerId);
        Assert.Equal(80, player.Gold); // 100 - 20
        Assert.Contains(CardId, player.Inventory);
        Assert.Equal(GameStateType.Shopping, state.State); // stays in Shopping
    }

    [Fact]
    public void GameStateMachine_OnBuyCardCommand_AllowsMultiplePurchases()
    {
        // Arrange
        var state = BuildState(gold: 100);
        var machine = new GameStateMachine(state);

        // Act
        machine.Handle(BuildCommand(cardId: CardId));
        machine.Handle(BuildCommand(cardId: "shrimp"));

        // Assert
        var player = state.Players.First(p => p.PlayerId == PlayerId);
        Assert.Equal(60, player.Gold); // 100 - 20 - 20
        Assert.Equal(2, player.Inventory.Count);
        Assert.Equal(GameStateType.Shopping, state.State);
    }

    // ── Rejection scenarios ───────────────────────────────────────────────────

    [Theory]
    [InlineData(GameStateType.PlayerTurn)]
    [InlineData(GameStateType.Initial)]
    [InlineData(GameStateType.VillainTurn)]
    public void GameStateMachine_OnBuyCardCommand_WhenWrongGameState_Rejects(GameStateType stateType)
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
    public void GameStateMachine_OnBuyCardCommand_WhenNotActivePlayer_Rejects()
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

    [Fact]
    public void GameStateMachine_OnBuyCardCommand_WhenCardNotInShop_Rejects()
    {
        // Arrange
        var state = BuildState();

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand(cardId: "scroll_of_kindle")).Events;

        // Assert
        Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
    }

    [Fact]
    public void GameStateMachine_OnBuyCardCommand_WhenInsufficientGold_Rejects()
    {
        // Arrange
        var state = BuildState(gold: 10); // card costs 20

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand()).Events;

        // Assert
        Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
    }
}
