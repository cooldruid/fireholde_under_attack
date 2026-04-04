using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class StartGameCommandSaga
{
    public static readonly CommandSaga<StartGameCommand> Saga = new CommandSaga<StartGameCommand>()
        .WhenIn(Initial)
        .Validate(HasAtLeastOnePlayer)
        .Execute(InitialiseTurn)
        .Emit(GameStarted)
        .TransitionTo(PlayerTurnStarting);

    private static bool HasAtLeastOnePlayer(StartGameCommand _, GameState state) =>
        state.Players.Count > 0;

    private static void InitialiseTurn(StartGameCommand _, GameState state)
    {
        var firstPlayer = state.Players[0];
        state.TurnMarker = new TurnMarker
        {
            ActivePlayerId = firstPlayer.PlayerId,
            ActivePlayerIndex = 0,
            ActionsRemaining = firstPlayer.ActionsPerTurn
        };
        state.Round = 1;
    }

    private static IEvent GameStarted(StartGameCommand _, GameState state) =>
        new GameStartedEvent
        {
            GameId = state.GameId,
            ActivePlayerId = state.TurnMarker!.ActivePlayerId,
            Round = state.Round
        };
}
