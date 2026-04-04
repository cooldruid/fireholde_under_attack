using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class PlayerActionEndingOnEnterSaga
{
    public static readonly CommandSaga<OnEnterCommand> Saga = new CommandSaga<OnEnterCommand>()
        .Execute(DecrementAndDetermineNextState)
        .TransitionTo((_, _, ctx) => ctx.Get<GameStateType>("nextState"));

    private static void DecrementAndDetermineNextState(OnEnterCommand _, GameState state, SagaContext ctx)
    {
        state.TurnMarker!.ActionsRemaining--;

        if (state.TurnMarker.ActionsRemaining > 0)
        {
            ctx.Set("nextState", PlayerTurn);
            return;
        }

        var isVillainTurn = TurnHelper.AdvanceToNextPlayerOrVillain(state);
        ctx.Set("nextState", isVillainTurn ? VillainTurn : PlayerTurnStarting);
    }
}
