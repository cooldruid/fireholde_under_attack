using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

/// <summary>
/// The single place where the turn cursor is mutated.
/// Call AdvancePlayerTurn at the end of every player-action saga.
/// Call AdvanceVillainTurn at the end of the villain-turn saga.
/// </summary>
internal static class TurnHelper
{
    public static void AdvancePlayerTurn(GameState state, SagaContext ctx)
    {
        var nextIndex = (state.ActivePlayerIndex + 1) % state.Players.Count;
        if (nextIndex == 0)
        {
            state.State = VillainTurn;
            state.ActivePlayerId = null;
            state.Round++;
            ctx.Set("isVillainTurn", true);
        }
        else
        {
            state.ActivePlayerIndex = nextIndex;
            state.ActivePlayerId = state.Players[nextIndex].Id;
            ctx.Set("isVillainTurn", false);
        }
    }

    public static void AdvanceVillainTurn(GameState state)
    {
        state.ActivePlayerIndex = 0;
        state.ActivePlayerId = state.Players[0].Id;
        state.State = PlayerTurn;
    }
}
