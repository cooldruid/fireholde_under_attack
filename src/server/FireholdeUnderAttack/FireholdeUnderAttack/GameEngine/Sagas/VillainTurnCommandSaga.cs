using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class VillainTurnCommandSaga
{
    public static readonly CommandSaga<VillainTurnCommand> Saga = new CommandSaga<VillainTurnCommand>()
        .WhenIn(VillainTurn)
        .Execute(ActAndAdvance)
        .Emit(TurnChanged);

    private static void ActAndAdvance(VillainTurnCommand _, GameState state)
    {
        // Villain AI logic goes here.
        TurnHelper.AdvanceVillainTurn(state);
    }

    private static IEvent TurnChanged(VillainTurnCommand cmd, GameState state) =>
        new TurnChangedEvent
        {
            GameId = state.GameId,
            ActivePlayerId = state.ActivePlayerId,
            IsVillainTurn = false,
            Round = state.Round
        };
}
