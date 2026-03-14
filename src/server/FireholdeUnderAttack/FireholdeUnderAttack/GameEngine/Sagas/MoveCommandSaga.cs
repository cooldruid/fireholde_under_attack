using FireholdeUnderAttack.Commands;
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
        .Execute((_, state, ctx) => TurnHelper.AdvancePlayerTurn(state, ctx))
        .Emit(PlayerMoved)
        .Emit(TurnChanged);

    private static bool PlayerExists(MoveCommand cmd, GameState state) =>
        state.Players.Any(p => p.Id == cmd.PlayerId);

    private static bool IsActivePlayer(MoveCommand cmd, GameState state) =>
        state.ActivePlayerId == cmd.PlayerId;

    private static void RollDiceAndMove(MoveCommand cmd, GameState state, SagaContext ctx)
    {
        var player = state.Players.First(p => p.Id == cmd.PlayerId);
        var dice = new Random().Next(1, 7);
        ctx.Set("dice", dice);
        player.CurrentTile += dice;
    }

    private static IEvent PlayerMoved(MoveCommand cmd, GameState state, SagaContext ctx) =>
        new MoveEvent
        {
            PlayerId = cmd.PlayerId,
            DiceRoll = ctx.Get<int>("dice"),
            NewTileId = state.Players.First(p => p.Id == cmd.PlayerId).CurrentTile
        };

    private static IEvent TurnChanged(MoveCommand cmd, GameState state, SagaContext ctx) =>
        new TurnChangedEvent
        {
            GameId = cmd.GameId,
            ActivePlayerId = state.ActivePlayerId,
            IsVillainTurn = ctx.Get<bool>("isVillainTurn"),
            Round = state.Round
        };
}
