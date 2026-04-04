using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Data;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;
using FireholdeUnderAttack.GameEngine.Saga;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class CommandSagaTests
{
    // A minimal command used only to exercise the saga builder
    private class TestCommand : ICommand { }

    private enum TestOutcome { A, B, C }

    private static GameState BuildState(GameStateType stateType = GameStateType.PlayerTurn)
    {
        var state = GameState.Create(Guid.NewGuid(), "Test");
        state.State = stateType;
        state.TurnMarker = new TurnMarker
        {
            ActivePlayerId = state.Players[0].PlayerId,
            ActivePlayerIndex = 0,
            ActionsRemaining = 3
        };
        return state;
    }

    // ── TransitionTo ──────────────────────────────────────────────────────────

    [Fact]
    public void TransitionTo_SetsStateAfterSagaCompletes()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .WhenIn(GameStateType.PlayerTurn)
            .TransitionTo(GameStateType.Shopping);

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Equal(GameStateType.Shopping, state.State);
    }

    [Fact]
    public void TransitionTo_ContextDriven_ResolvesCorrectState()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => ctx.Set("next", GameStateType.Shopping))
            .TransitionTo((_, _, ctx) => ctx.Get<GameStateType>("next"));

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Equal(GameStateType.Shopping, state.State);
    }

    [Fact]
    public void TransitionTo_NotApplied_WhenGuardRejects()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .WhenIn(GameStateType.VillainTurn) // guard will fail
            .TransitionTo(GameStateType.Shopping);

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert — state unchanged
        Assert.Equal(GameStateType.PlayerTurn, state.State);
    }

    [Fact]
    public void TransitionTo_WhenMultipleSteps_LastOneWins()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .TransitionTo(GameStateType.Shopping)
            .TransitionTo(GameStateType.TreasureRoom);

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Equal(GameStateType.TreasureRoom, state.State);
    }

    // ── Conditional Emit ──────────────────────────────────────────────────────

    [Fact]
    public void ConditionalEmit_EmitsWhenPredicateTrue()
    {
        // Arrange
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Emit(
                (_, s) => new CommandRejectedEvent { GameId = s.GameId, Reason = "test" },
                when: (_, _, _) => true);

        // Act
        var events = saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Single(events);
    }

    [Fact]
    public void ConditionalEmit_SkipsWhenPredicateFalse()
    {
        // Arrange
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Emit(
                (_, s) => new CommandRejectedEvent { GameId = s.GameId, Reason = "test" },
                when: (_, _, _) => false);

        // Act
        var events = saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Empty(events);
    }

    [Fact]
    public void ConditionalEmit_ContextDriven_EmitsOnlyWhenFlagSet()
    {
        // Arrange
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => ctx.Set("emit", true))
            .Emit(
                (_, s) => new CommandRejectedEvent { GameId = s.GameId, Reason = "test" },
                when: (_, _, ctx) => ctx.Get<bool>("emit"));

        // Act
        var events = saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Single(events);
    }

    // ── Branch ────────────────────────────────────────────────────────────────

    [Fact]
    public void Branch_ExecutesMatchingCase()
    {
        // Arrange
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => ctx.Set("outcome", TestOutcome.A))
            .Branch((_, _, ctx) => ctx.Get<TestOutcome>("outcome"))
                .Case(TestOutcome.A, s => s.TransitionTo(GameStateType.Shopping))
                .Default(s => s.TransitionTo(GameStateType.VillainTurn));

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Equal(GameStateType.Shopping, state.State);
    }

    [Fact]
    public void Branch_ExecutesDefault_WhenNoMatchingCase()
    {
        // Arrange
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => ctx.Set("outcome", TestOutcome.B))
            .Branch((_, _, ctx) => ctx.Get<TestOutcome>("outcome"))
                .Case(TestOutcome.A, s => s.TransitionTo(GameStateType.Shopping))
                .Default(s => s.TransitionTo(GameStateType.VillainTurn));

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Equal(GameStateType.VillainTurn, state.State);
    }

    [Fact]
    public void Branch_EmitsEventFromMatchingCase()
    {
        // Arrange
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => ctx.Set("outcome", TestOutcome.A))
            .Branch((_, _, ctx) => ctx.Get<TestOutcome>("outcome"))
                .Case(TestOutcome.A, s => s
                    .Emit((_, st) => new CommandRejectedEvent { GameId = st.GameId, Reason = "case-a" })
                    .TransitionTo(GameStateType.Shopping))
                .Default(s => s
                    .Emit((_, st) => new CommandRejectedEvent { GameId = st.GameId, Reason = "default" })
                    .TransitionTo(GameStateType.VillainTurn));

        // Act
        var events = saga.Execute(new TestCommand(), state);

        // Assert
        var evt = Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
        Assert.Equal("case-a", evt.Reason);
    }

    [Fact]
    public void Branch_DoesNotExecuteOtherCases()
    {
        // Arrange
        var mutationCount = 0;
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => ctx.Set("outcome", TestOutcome.A))
            .Branch((_, _, ctx) => ctx.Get<TestOutcome>("outcome"))
                .Case(TestOutcome.A, s => s.Execute((_, _, _) => mutationCount++))
                .Default(s => s.Execute((_, _, _) => mutationCount += 100));

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert — only case A ran
        Assert.Equal(1, mutationCount);
    }

    [Fact]
    public void Branch_GuardRejection_ShortCircuitsInsideBranchCase()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => ctx.Set("outcome", TestOutcome.A))
            .Branch((_, _, ctx) => ctx.Get<TestOutcome>("outcome"))
                .Case(TestOutcome.A, s => s
                    .WhenIn(GameStateType.VillainTurn) // guard fails
                    .TransitionTo(GameStateType.Shopping))
                .Default(s => s.TransitionTo(GameStateType.VillainTurn));

        // Act
        var events = saga.Execute(new TestCommand(), state);

        // Assert — guard inside case rejected; TransitionTo(Shopping) was not applied
        Assert.Single(events); // CommandRejectedEvent from inner guard
        Assert.IsType<CommandRejectedEvent>(events[0]);
        Assert.Equal(GameStateType.PlayerTurn, state.State); // no transition
    }

    [Fact]
    public void Branch_NestedBranch_ExecutesCorrectLeafCase()
    {
        // Arrange
        var state = BuildState();
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .Execute((_, _, ctx) => { ctx.Set("outer", TestOutcome.A); ctx.Set("inner", TestOutcome.C); })
            .Branch((_, _, ctx) => ctx.Get<TestOutcome>("outer"))
                .Case(TestOutcome.A, s => s
                    .Branch((_, _, ctx) => ctx.Get<TestOutcome>("inner"))
                        .Case(TestOutcome.C, inner => inner.TransitionTo(GameStateType.Shopping))
                        .Default(inner => inner.TransitionTo(GameStateType.VillainTurn)))
                .Default(s => s.TransitionTo(GameStateType.TreasureRoom));

        // Act
        saga.Execute(new TestCommand(), state);

        // Assert
        Assert.Equal(GameStateType.Shopping, state.State);
    }

    // ── Guard behaviour ───────────────────────────────────────────────────────

    [Fact]
    public void Guard_ShortCircuits_ReturnsSingleRejectionEvent()
    {
        // Arrange
        var state = BuildState(GameStateType.PlayerTurn);
        var mutationRan = false;
        ICommandSaga saga = new CommandSaga<TestCommand>()
            .WhenIn(GameStateType.VillainTurn)
            .Execute((_, _, _) => mutationRan = true);

        // Act
        var events = saga.Execute(new TestCommand(), state);

        // Assert
        Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
        Assert.False(mutationRan);
    }
}
