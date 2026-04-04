using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class PlayerTurnStartingOnEnterSaga
{
    public static readonly CommandSaga<OnEnterCommand> Saga = new CommandSaga<OnEnterCommand>()
        .Emit(TurnChanged)
        .TransitionTo(PlayerTurn);

    private static IEvent TurnChanged(OnEnterCommand _, GameState state) =>
        new TurnChangedEvent
        {
            GameId = state.GameId,
            ActivePlayerId = state.TurnMarker!.ActivePlayerId,
            IsVillainTurn = false,
            Round = state.Round
        };
}
