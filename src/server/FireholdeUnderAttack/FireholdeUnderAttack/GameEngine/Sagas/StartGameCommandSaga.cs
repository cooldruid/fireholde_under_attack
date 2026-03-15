using FireholdeUnderAttack.Commands;
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
        .Emit(GameStarted);

    private static bool HasAtLeastOnePlayer(StartGameCommand _, GameState state) =>
        state.Players.Count > 0;

    private static void InitialiseTurn(StartGameCommand _, GameState state)
    {
        state.ActivePlayerIndex = 0;
        state.ActivePlayerId = state.Players[0].PlayerId;
        state.Round = 1;
        state.State = PlayerTurn;
    }

    private static IEvent GameStarted(StartGameCommand _, GameState state) =>
        new GameStartedEvent
        {
            GameId = state.GameId,
            ActivePlayerId = state.Players[0].PlayerId,
            Round = state.Round
        };
}
