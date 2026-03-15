using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using FireholdeUnderAttack.GameEngine.Sagas;

namespace FireholdeUnderAttack.GameEngine;

public class GameStateMachine(GameState gameState)
{
    public GameState State => gameState;

    private static readonly Dictionary<Type, ICommandSaga> Sagas = new()
    {
        [typeof(MoveCommand)] = MoveCommandSaga.Saga,
        [typeof(StartGameCommand)] = StartGameCommandSaga.Saga,
        [typeof(JoinGameCommand)] = JoinGameCommandSaga.Saga,
        [typeof(VillainTurnCommand)] = VillainTurnCommandSaga.Saga,
    };

    public List<IEvent> Handle(ICommand command)
    {
        if (!Sagas.TryGetValue(command.GetType(), out var saga))
            throw new NotImplementedException($"No saga registered for command '{command.GetType().Name}'");

        return saga.Execute(command, gameState);
    }
}
