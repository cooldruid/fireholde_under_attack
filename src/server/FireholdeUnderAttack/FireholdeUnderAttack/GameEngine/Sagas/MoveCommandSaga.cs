using FireholdeUnderAttack.Catalogs.Classes;
using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Constants;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class MoveCommandSaga
{
    public static readonly CommandSaga<MoveCommand> Saga = new CommandSaga<MoveCommand>()
        .WhenIn(PlayerTurn)
        .Validate(PlayerExists)
        .Validate(IsActivePlayer)
        .Execute(RollDiceAndMove)
        .Execute(ApplyOnMovePassives)
        .Branch(GetLandedTileType)
            .Case(BoardTileType.Shop,
                saga => saga
                    .Emit(PlayerMoved)
                    .TransitionTo(Shopping))
            .Default(
                saga => saga
                    .Emit(PlayerMoved)
                    .TransitionTo(PlayerActionEnding));

    private static bool PlayerExists(MoveCommand cmd, GameState state) =>
        state.Players.Any(p => p.PlayerId == cmd.PlayerId);

    private static bool IsActivePlayer(MoveCommand cmd, GameState state) =>
        state.TurnMarker?.ActivePlayerId == cmd.PlayerId;

    private static void RollDiceAndMove(MoveCommand cmd, GameState state, SagaContext ctx)
    {
        var player = state.Players.First(p => p.PlayerId == cmd.PlayerId);
        var dice = new Random().Next(1, 7);
        ctx.Set("dice", dice);
        player.CurrentTile = (player.CurrentTile + dice) % state.Board.Tiles.Count;
    }

    private static void ApplyOnMovePassives(MoveCommand cmd, GameState state, SagaContext ctx)
    {
        var player = state.Players.First(p => p.PlayerId == cmd.PlayerId);
        foreach (var cardId in player.Inventory.ToList())
        {
            if (CardCatalog.All.TryGetValue(cardId, out var card))
                card.OnMove?.Invoke(state, cmd.PlayerId, null);
        }

        var landedTile = state.Board.Tiles[player.CurrentTile];
        if (landedTile.Effects.Remove(TileEffects.ElementalSoul))
        {
            player.AttackDamage += 1;
            ctx.Set("collectedSoul", true);
            ClassCatalog.SpawnElementalSoul(state);
        }
    }

    private static BoardTileType GetLandedTileType(MoveCommand cmd, GameState state, SagaContext ctx)
    {
        var player = state.Players.First(p => p.PlayerId == cmd.PlayerId);
        return state.Board.Tiles[player.CurrentTile].Type;
    }

    private static IEvent PlayerMoved(MoveCommand cmd, GameState state, SagaContext ctx) =>
        new MoveEvent
        {
            PlayerId = cmd.PlayerId,
            DiceRoll = ctx.Get<int>("dice"),
            NewTileId = state.Players.First(p => p.PlayerId == cmd.PlayerId).CurrentTile
        };
}
