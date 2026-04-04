using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Constants;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class ShoppingOnEnterSaga
{
    public static readonly CommandSaga<OnEnterCommand> Saga = new CommandSaga<OnEnterCommand>()
        .Execute(PopulateShopInventory)
        .Emit(ShopOpened);

    private static void PopulateShopInventory(OnEnterCommand _, GameState state, SagaContext ctx)
    {
        var activePlayer = state.Players.First(p => p.PlayerId == state.TurnMarker!.ActivePlayerId);
        var level = activePlayer.Level;
        const int maxLevel = 5;

        var primary = CardCatalog.All.Values
            .Where(c => c.Level == level)
            .OrderBy(_ => Random.Shared.Next())
            .Take(3)
            .Select(c => c.Id)
            .ToList();

        var upgradeLevel = Math.Min(level + 1, maxLevel);
        var upgrade = CardCatalog.All.Values
            .Where(c => c.Level == upgradeLevel)
            .OrderBy(_ => Random.Shared.Next())
            .Take(1)
            .Select(c => c.Id)
            .ToList();

        state.ShopInventory = [.. primary, .. upgrade];
    }

    private static IEvent ShopOpened(OnEnterCommand _, GameState state) =>
        new ShopOpenedEvent
        {
            GameId = state.GameId,
            PlayerId = state.TurnMarker!.ActivePlayerId!.Value,
            AvailableCards = state.ShopInventory
                .Select(id => CardCatalog.All[id])
                .Select(c => new ShopCardInfo(c.Id, c.Name, c.Description, c.Price, c.Level))
                .ToList()
        };
}
