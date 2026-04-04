namespace FireholdeUnderAttack.GameEngine.Sagas;

/// <summary>
/// Shared turn-cursor helpers called from sagas.
/// Does NOT set state.State — sagas use TransitionTo for that.
/// </summary>
internal static class TurnHelper
{
    /// <summary>
    /// Advances the cursor to the next player, or sets up villain turn if all players have gone.
    /// Returns true if transitioning to villain turn.
    /// </summary>
    public static bool AdvanceToNextPlayerOrVillain(GameState state)
    {
        var nextIndex = (state.TurnMarker!.ActivePlayerIndex + 1) % state.Players.Count;
        if (nextIndex == 0)
        {
            state.TurnMarker.ActivePlayerId = null;
            state.TurnMarker.ActivePlayerIndex = -1;
            state.Round++;
            return true;
        }

        var nextPlayer = state.Players.First(p => p.PlayerIndex == nextIndex);
        state.TurnMarker.ActivePlayerId = nextPlayer.PlayerId;
        state.TurnMarker.ActivePlayerIndex = nextIndex;
        state.TurnMarker.ActionsRemaining = nextPlayer.ActionsPerTurn;
        return false;
    }

    /// <summary>
    /// Resets the turn cursor to player 0 at the start of the player phase after villain turn.
    /// </summary>
    public static void AdvanceVillainTurn(GameState state)
    {
        var firstPlayer = state.Players.First(p => p.PlayerIndex == 0);
        state.TurnMarker = new Data.TurnMarker
        {
            ActivePlayerId = firstPlayer.PlayerId,
            ActivePlayerIndex = 0,
            ActionsRemaining = firstPlayer.ActionsPerTurn
        };
    }
}
