using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;

namespace FireholdeUnderAttack.GameEngine.Saga;

internal interface ICommandSaga
{
    List<IEvent> Execute(ICommand command, GameState state);
}
