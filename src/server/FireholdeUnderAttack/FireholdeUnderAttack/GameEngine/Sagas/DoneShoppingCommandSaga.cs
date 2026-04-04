using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class DoneShoppingCommandSaga
{
    public static readonly CommandSaga<DoneShoppingCommand> Saga = new CommandSaga<DoneShoppingCommand>()
        .WhenIn(Shopping)
        .Validate(PlayerExists)
        .Validate(IsActivePlayer)
        .Execute(ClearShopInventory)
        .TransitionTo(PlayerActionEnding);

    private static bool PlayerExists(DoneShoppingCommand cmd, GameState state) =>
        state.Players.Any(p => p.PlayerId == cmd.PlayerId);

    private static bool IsActivePlayer(DoneShoppingCommand cmd, GameState state) =>
        state.TurnMarker?.ActivePlayerId == cmd.PlayerId;

    private static void ClearShopInventory(DoneShoppingCommand _, GameState state)
    {
        state.ShopInventory = [];
    }
}
