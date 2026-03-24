using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class JoinGameCommandSaga
{
    private const int MaxPlayers = 4;

    public static readonly CommandSaga<JoinGameCommand> Saga = new CommandSaga<JoinGameCommand>()
        .WhenIn(Initial)
        .Validate(NotAlreadyInGame)
        .Validate(GameNotFull)
        .Execute(AddPlayer)
        .Emit(PlayerJoined);

    private static bool NotAlreadyInGame(JoinGameCommand cmd, GameState state) =>
        state.Players.All(p => p.PlayerId != cmd.PlayerId);

    private static bool GameNotFull(JoinGameCommand _, GameState state) =>
        state.Players.Count < MaxPlayers;

    private static void AddPlayer(JoinGameCommand cmd, GameState state)
    {
        var index = state.Players.Max(p => p.PlayerIndex) + 1;
        state.Players.Add(Player.Create(cmd.PlayerId, index, cmd.PlayerName));
    }

    private static IEvent PlayerJoined(JoinGameCommand cmd, GameState state) =>
        new PlayerJoinedEvent
        {
            GameId = state.GameId,
            PlayerId = cmd.PlayerId,
            PlayerName = cmd.PlayerName
        };
}
