using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Constants;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class BuyCardCommandSaga
{
    public static readonly CommandSaga<BuyCardCommand> Saga = new CommandSaga<BuyCardCommand>()
        .WhenIn(Shopping)
        .Validate(PlayerExists)
        .Validate(IsActivePlayer)
        .Validate(CardIsInShop)
        .Validate(CanAfford)
        .Execute(Purchase)
        .Emit(CardAcquired);

    private static bool PlayerExists(BuyCardCommand cmd, GameState state) =>
        state.Players.Any(p => p.PlayerId == cmd.PlayerId);

    private static bool IsActivePlayer(BuyCardCommand cmd, GameState state) =>
        state.TurnMarker?.ActivePlayerId == cmd.PlayerId;

    private static bool CardIsInShop(BuyCardCommand cmd, GameState state) =>
        state.ShopInventory.Contains(cmd.CardId);

    private static bool CanAfford(BuyCardCommand cmd, GameState state)
    {
        var player = state.Players.First(p => p.PlayerId == cmd.PlayerId);
        return player.Gold >= CardCatalog.All[cmd.CardId].Price;
    }

    private static void Purchase(BuyCardCommand cmd, GameState state, SagaContext ctx)
    {
        var player = state.Players.First(p => p.PlayerId == cmd.PlayerId);
        var card = CardCatalog.All[cmd.CardId];
        player.Gold -= card.Price;
        player.Inventory.Add(cmd.CardId);
    }

    private static IEvent CardAcquired(BuyCardCommand cmd, GameState state, SagaContext ctx) =>
        new CardAcquiredEvent
        {
            GameId = state.GameId,
            PlayerId = cmd.PlayerId,
            CardId = cmd.CardId
        };
}
