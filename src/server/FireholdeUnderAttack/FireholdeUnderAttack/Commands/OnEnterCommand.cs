namespace FireholdeUnderAttack.Commands;

/// <summary>
/// Shared internal command enqueued automatically whenever a state with a registered
/// on-enter saga is entered. Sagas consume it via CommandSaga&lt;OnEnterCommand&gt; and are
/// registered in GameStateMachine.OnEnterSagas — no per-state command class needed.
/// </summary>
internal sealed class OnEnterCommand : ICommand { }
