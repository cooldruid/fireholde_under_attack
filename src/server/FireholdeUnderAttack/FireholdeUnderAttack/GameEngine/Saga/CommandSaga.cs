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
            (cmd, state, _) => new CommandRejectedEvent
            {
                GameId = state.GameId,
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
            (cmd, state, _) => new CommandRejectedEvent { GameId = state.GameId, Reason = $"{typeof(TCommand).Name} rejected: {reason}" }
        ));
        return this;
    }

    public CommandSaga<TCommand> Validate(
        Func<TCommand, GameState, bool> predicate,
        [CallerArgumentExpression(nameof(predicate))] string? reason = null)
    {
        _steps.Add(new GuardStep(
            (cmd, state, _) => predicate(cmd, state),
            (cmd, state, _) => new CommandRejectedEvent { GameId = state.GameId, Reason = $"{typeof(TCommand).Name} rejected: {reason}" }
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

    public CommandSaga<TCommand> Emit(
        Func<TCommand, GameState, SagaContext, IEvent> factory,
        Func<TCommand, GameState, SagaContext, bool> when)
    {
        _steps.Add(new EmitStep(factory, when));
        return this;
    }

    public CommandSaga<TCommand> Emit(
        Func<TCommand, GameState, IEvent> factory,
        Func<TCommand, GameState, SagaContext, bool> when)
    {
        _steps.Add(new EmitStep((cmd, state, _) => factory(cmd, state), when));
        return this;
    }

    // ── State transition ──────────────────────────────────────────────────────

    public CommandSaga<TCommand> TransitionTo(GameStateType nextState)
    {
        _steps.Add(new TransitionStep((_, _, _) => nextState));
        return this;
    }

    public CommandSaga<TCommand> TransitionTo(Func<TCommand, GameState, SagaContext, GameStateType> selector)
    {
        _steps.Add(new TransitionStep(selector));
        return this;
    }

    // ── Branching ─────────────────────────────────────────────────────────────

    public BranchBuilder<TKey> Branch<TKey>(Func<TCommand, GameState, SagaContext, TKey> selector)
        where TKey : notnull
    {
        var step = new BranchStep((cmd, state, ctx) => (object)selector(cmd, state, ctx)!);
        _steps.Add(step);
        return new BranchBuilder<TKey>(this, step);
    }

    // ── Execution ─────────────────────────────────────────────────────────────

    List<IEvent> ICommandSaga.Execute(ICommand command, GameState state)
    {
        var cmd = (TCommand)command;
        var ctx = new SagaContext();
        var events = new List<IEvent>();
        GameStateType? pendingTransition = null;

        var rejection = ExecuteSteps(_steps, cmd, state, ctx, events, ref pendingTransition);
        if (rejection != null) return [rejection];

        if (pendingTransition.HasValue)
            state.State = pendingTransition.Value;

        return events;
    }

    private static IEvent? ExecuteSteps(
        List<SagaStep> steps,
        TCommand cmd,
        GameState state,
        SagaContext ctx,
        List<IEvent> events,
        ref GameStateType? pendingTransition)
    {
        foreach (var step in steps)
        {
            switch (step)
            {
                case GuardStep guard:
                    if (!guard.Predicate(cmd, state, ctx))
                        return guard.OnFailure(cmd, state, ctx);
                    break;

                case MutateStep mutate:
                    mutate.Action(cmd, state, ctx);
                    break;

                case EmitStep emit:
                    if (emit.When == null || emit.When(cmd, state, ctx))
                        events.Add(emit.Factory(cmd, state, ctx));
                    break;

                case TransitionStep transition:
                    pendingTransition = transition.Selector(cmd, state, ctx);
                    break;

                case BranchStep branch:
                    var key = branch.Selector(cmd, state, ctx);
                    var branchSteps = branch.Cases.TryGetValue(key, out var found)
                        ? found
                        : branch.DefaultSteps;
                    if (branchSteps != null)
                    {
                        var branchRejection = ExecuteSteps(branchSteps, cmd, state, ctx, events, ref pendingTransition);
                        if (branchRejection != null) return branchRejection;
                    }
                    break;
            }
        }
        return null;
    }

    // ── Step types ────────────────────────────────────────────────────────────

    private abstract class SagaStep { }

    private sealed class GuardStep(
        Func<TCommand, GameState, SagaContext, bool> predicate,
        Func<TCommand, GameState, SagaContext, IEvent> onFailure) : SagaStep
    {
        public Func<TCommand, GameState, SagaContext, bool> Predicate { get; } = predicate;
        public Func<TCommand, GameState, SagaContext, IEvent> OnFailure { get; } = onFailure;
    }

    private sealed class MutateStep(Action<TCommand, GameState, SagaContext> action) : SagaStep
    {
        public Action<TCommand, GameState, SagaContext> Action { get; } = action;
    }

    private sealed class EmitStep(
        Func<TCommand, GameState, SagaContext, IEvent> factory,
        Func<TCommand, GameState, SagaContext, bool>? when = null) : SagaStep
    {
        public Func<TCommand, GameState, SagaContext, IEvent> Factory { get; } = factory;
        public Func<TCommand, GameState, SagaContext, bool>? When { get; } = when;
    }

    private sealed class TransitionStep(Func<TCommand, GameState, SagaContext, GameStateType> selector) : SagaStep
    {
        public Func<TCommand, GameState, SagaContext, GameStateType> Selector { get; } = selector;
    }

    private sealed class BranchStep(Func<TCommand, GameState, SagaContext, object> selector) : SagaStep
    {
        public Func<TCommand, GameState, SagaContext, object> Selector { get; } = selector;
        public Dictionary<object, List<SagaStep>> Cases { get; } = new();
        public List<SagaStep>? DefaultSteps { get; set; }
    }

    // ── Branch builder ────────────────────────────────────────────────────────

    public sealed class BranchBuilder<TKey> where TKey : notnull
    {
        private readonly CommandSaga<TCommand> _parent;
        private readonly BranchStep _step;

        internal BranchBuilder(CommandSaga<TCommand> parent, object step)
        {
            _parent = parent;
            _step = (BranchStep)step;
        }

        public BranchBuilder<TKey> Case(TKey key, Func<CommandSaga<TCommand>, CommandSaga<TCommand>> configure)
        {
            var sub = new CommandSaga<TCommand>();
            configure(sub);
            _step.Cases[(object)key!] = sub._steps;
            return this;
        }

        public CommandSaga<TCommand> Default(Func<CommandSaga<TCommand>, CommandSaga<TCommand>> configure)
        {
            var sub = new CommandSaga<TCommand>();
            configure(sub);
            _step.DefaultSteps = sub._steps;
            return _parent;
        }
    }
}
