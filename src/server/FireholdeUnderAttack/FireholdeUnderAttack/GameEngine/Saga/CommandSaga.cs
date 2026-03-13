using System.Runtime.CompilerServices;
using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;

namespace FireholdeUnderAttack.GameEngine.Saga;

public sealed class CommandSaga<TCommand> : ICommandSaga where TCommand : ICommand
{
    private readonly List<SagaStep> _steps = [];

    // ── Guards ────────────────────────────────────────────────────────────────

    public CommandSaga<TCommand> WhenIn(params GameStateType[] states)
    {
        _steps.Add(new GuardStep(
            (_, state, _) => states.Contains(state.State),
            (cmd, _, _) => new CommandRejectedEvent
            {
                GameId = cmd.GameId,
                Reason = $"{typeof(TCommand).Name} not valid in state '{string.Join(", ", states.Select(s => s.ToString()))}'"
            }
        ));
        return this;
    }

    public CommandSaga<TCommand> Validate(
        Func<TCommand, GameState, SagaContext, bool> predicate,
        [CallerArgumentExpression(nameof(predicate))] string? reason = null)
    {
        _steps.Add(new GuardStep(
            predicate,
            (cmd, _, _) => new CommandRejectedEvent { GameId = cmd.GameId, Reason = $"{typeof(TCommand).Name} rejected: {reason}" }
        ));
        return this;
    }

    public CommandSaga<TCommand> Validate(
        Func<TCommand, GameState, bool> predicate,
        [CallerArgumentExpression(nameof(predicate))] string? reason = null)
    {
        _steps.Add(new GuardStep(
            (cmd, state, _) => predicate(cmd, state),
            (cmd, _, _) => new CommandRejectedEvent { GameId = cmd.GameId, Reason = $"{typeof(TCommand).Name} rejected: {reason}" }
        ));
        return this;
    }

    // ── State mutation ────────────────────────────────────────────────────────

    public CommandSaga<TCommand> Execute(Action<TCommand, GameState, SagaContext> action)
    {
        _steps.Add(new MutateStep(action));
        return this;
    }

    public CommandSaga<TCommand> Execute(Action<TCommand, GameState> action)
    {
        _steps.Add(new MutateStep((cmd, state, _) => action(cmd, state)));
        return this;
    }

    // ── Event emission ────────────────────────────────────────────────────────

    public CommandSaga<TCommand> Emit(Func<TCommand, GameState, SagaContext, IEvent> factory)
    {
        _steps.Add(new EmitStep(factory));
        return this;
    }

    public CommandSaga<TCommand> Emit(Func<TCommand, GameState, IEvent> factory)
    {
        _steps.Add(new EmitStep((cmd, state, _) => factory(cmd, state)));
        return this;
    }

    // ── Execution ─────────────────────────────────────────────────────────────

    List<IEvent> ICommandSaga.Execute(ICommand command, GameState state)
    {
        var cmd = (TCommand)command;
        var ctx = new SagaContext();
        var events = new List<IEvent>();

        foreach (var step in _steps)
        {
            switch (step)
            {
                case GuardStep guard:
                    if (!guard.Predicate(cmd, state, ctx))
                        return [guard.OnFailure(cmd, state, ctx)];
                    break;

                case MutateStep mutate:
                    mutate.Action(cmd, state, ctx);
                    break;

                case EmitStep emit:
                    events.Add(emit.Factory(cmd, state, ctx));
                    break;
            }
        }

        return events;
    }

    // ── Step types ────────────────────────────────────────────────────────────

    private abstract record SagaStep;

    private sealed record GuardStep(
        Func<TCommand, GameState, SagaContext, bool> Predicate,
        Func<TCommand, GameState, SagaContext, IEvent> OnFailure) : SagaStep;

    private sealed record MutateStep(
        Action<TCommand, GameState, SagaContext> Action) : SagaStep;

    private sealed record EmitStep(
        Func<TCommand, GameState, SagaContext, IEvent> Factory) : SagaStep;
}
