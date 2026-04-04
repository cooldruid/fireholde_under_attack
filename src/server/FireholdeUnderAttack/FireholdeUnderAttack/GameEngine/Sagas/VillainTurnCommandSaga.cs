using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class VillainTurnCommandSaga
{
    public static readonly CommandSaga<VillainTurnCommand> Saga = new CommandSaga<VillainTurnCommand>()
        .WhenIn(VillainTurn)
        .Execute(Act)
        .TransitionTo(PlayerTurnStarting);

    private static void Act(VillainTurnCommand _, GameState state)
    {
        // Villain AI logic goes here.
        TurnHelper.AdvanceVillainTurn(state);
    }
}
