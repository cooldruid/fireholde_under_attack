using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using FireholdeUnderAttack.GameEngine.Sagas;

namespace FireholdeUnderAttack.GameEngine;

public record HandleResult(List<IEvent> Events, ICommand? OnEnterCommand);

public class GameStateMachine(GameState gameState)
{
    public GameState State => gameState;

    private static readonly Dictionary<Type, ICommandSaga> Sagas = new()
    {
        [typeof(MoveCommand)] = MoveCommandSaga.Saga,
        [typeof(StartGameCommand)] = StartGameCommandSaga.Saga,
        [typeof(JoinGameCommand)] = JoinGameCommandSaga.Saga,
        [typeof(VillainTurnCommand)] = VillainTurnCommandSaga.Saga,
        [typeof(BuyCardCommand)] = BuyCardCommandSaga.Saga,
        [typeof(DoneShoppingCommand)] = DoneShoppingCommandSaga.Saga,
        [typeof(UseCardCommand)] = UseCardCommandSaga.Saga,
    };

    /// <summary>
    /// On-enter sagas, keyed by the state being entered. Register a
    /// <see cref="CommandSaga{OnEnterCommand}"/> here — no WhenIn guard needed,
    /// the dispatch guarantees the correct state is active.
    /// </summary>
    internal static readonly Dictionary<GameStateType, ICommandSaga> OnEnterSagas = new()
    {
        [GameStateType.PlayerTurnStarting] = PlayerTurnStartingOnEnterSaga.Saga,
        [GameStateType.PlayerActionEnding] = PlayerActionEndingOnEnterSaga.Saga,
        [GameStateType.Shopping] = ShoppingOnEnterSaga.Saga,
    };

    public HandleResult Handle(ICommand command)
    {
        ICommandSaga saga;

        if (command is OnEnterCommand)
        {
            if (!OnEnterSagas.TryGetValue(gameState.State, out saga!))
                return new HandleResult([], null);
        }
        else
        {
            if (!Sagas.TryGetValue(command.GetType(), out saga!))
                throw new NotImplementedException($"No saga registered for command '{command.GetType().Name}'");
        }

        var previousState = gameState.State;
        var events = saga.Execute(command, gameState);
        var newState = gameState.State;

        ICommand? onEnterCommand = null;
        if (newState != previousState && OnEnterSagas.ContainsKey(newState))
            onEnterCommand = new OnEnterCommand();

        return new HandleResult(events, onEnterCommand);
    }
}
