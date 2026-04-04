using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class ShoppingOnEnterTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid PlayerId = Guid.NewGuid();

    private static GameState BuildState(int playerLevel = 1)
    {
        var state = GameState.Create(PlayerId, "Player 1");
        state.GameId = GameId;
        state.State = GameStateType.Shopping;
        state.TurnMarker = new TurnMarker
        {
            ActivePlayerId = PlayerId,
            ActivePlayerIndex = 0,
            ActionsRemaining = 3
        };
        state.Players.First().Level = playerLevel;
        return state;
    }

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnEnterShopping_EmitsShopOpenedEventWithCardInfo()
    {
        // Arrange
        var state = BuildState(playerLevel: 1);

        // Act
        var events = new GameStateMachine(state).Handle(new OnEnterCommand()).Events;

        // Assert
        var shopOpened = Assert.IsType<ShopOpenedEvent>(Assert.Single(events));
        Assert.Equal(GameId, shopOpened.GameId);
        Assert.Equal(PlayerId, shopOpened.PlayerId);
        Assert.NotEmpty(shopOpened.AvailableCards);
        Assert.All(shopOpened.AvailableCards, card =>
        {
            Assert.False(string.IsNullOrEmpty(card.Id));
            Assert.False(string.IsNullOrEmpty(card.Name));
            Assert.False(string.IsNullOrEmpty(card.Description));
            Assert.True(card.Price > 0);
        });
    }

    [Fact]
    public void GameStateMachine_OnEnterShopping_PopulatesShopInventoryWithUpToFourCards()
    {
        // Arrange
        var state = BuildState(playerLevel: 1);

        // Act
        new GameStateMachine(state).Handle(new OnEnterCommand());

        // Assert — 3 level-1 cards + 1 level-2 card (catalog has enough of each)
        Assert.Equal(4, state.ShopInventory.Count);
    }

    [Fact]
    public void GameStateMachine_OnEnterShopping_ShopInventoryMatchesEmittedCards()
    {
        // Arrange
        var state = BuildState(playerLevel: 1);

        // Act
        var events = new GameStateMachine(state).Handle(new OnEnterCommand()).Events;

        // Assert — event card IDs match what was stored in ShopInventory
        var shopOpened = Assert.IsType<ShopOpenedEvent>(Assert.Single(events));
        var emittedIds = shopOpened.AvailableCards.Select(c => c.Id).ToList();
        Assert.Equal(state.ShopInventory, emittedIds);
    }

    [Fact]
    public void GameStateMachine_OnEnterShopping_CardsAreFromCorrectLevels()
    {
        // Arrange
        var state = BuildState(playerLevel: 2);

        // Act
        new GameStateMachine(state).Handle(new OnEnterCommand());

        // Assert — first 3 should be level 2, last 1 should be level 3
        // (We check via CardCatalog since ShopInventory holds IDs)
        var catalog = Constants.CardCatalog.All;
        var levels = state.ShopInventory.Select(id => catalog[id].Level).ToList();
        Assert.Equal(3, levels.Count(l => l == 2));
        Assert.Equal(1, levels.Count(l => l == 3));
    }

    [Fact]
    public void GameStateMachine_OnEnterShopping_AtMaxLevel_AllCardsAreMaxLevel()
    {
        // Arrange
        var state = BuildState(playerLevel: 5);

        // Act
        new GameStateMachine(state).Handle(new OnEnterCommand());

        // Assert — level+1 would be 6 which is capped to 5, so all cards are level 5
        var catalog = Constants.CardCatalog.All;
        Assert.All(state.ShopInventory, id => Assert.Equal(5, catalog[id].Level));
    }
}
