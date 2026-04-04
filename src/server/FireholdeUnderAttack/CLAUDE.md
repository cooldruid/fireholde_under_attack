# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run (dev, port 5048)
dotnet run --project FireholdeUnderAttack/FireholdeUnderAttack.csproj

# Run tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~MoveCommandTests"
```

## Description

Fireholde Under Attack is a 1-4 player board game. It is a cooperative game where each player plays as a wizard. They aim to defeat a boss which has different mechanics every game. Each player has health and money as well as cards with items and powers they get during the gameplay. On the board there are different tiles such as shops, treasures, etc.

The rules of the game are not completely figured out so code that allows for rapid game rule changes is important.

## Architecture

ASP.NET Core 10.0 real-time multiplayer game server using a **command → saga → event** pipeline with SignalR for client broadcasting. Game state lives in-process for the lifetime of a game (no external storage).

### Request flow

1. Client posts to `POST /games/{gameId}/commands` with a `$type` discriminator in the JSON body
2. `GameEndpoints` looks up the `GameInstance` and enqueues the command
3. `GameInstance.ProcessLoop()` (background task per game, backed by `System.Threading.Channels`) dequeues and passes to `GameStateMachine`
4. `GameStateMachine` looks up the registered `CommandSaga<TCommand>` for the command type and runs it
5. The saga executes its steps in order: state guards → validation → state mutation → event emission
6. `GameInstance` broadcasts each event to the SignalR group for that game via `IHubContext<EventHub>`

### Saga pattern

Each command is handled by a `CommandSaga<TCommand>` defined in `GameEngine/Sagas/`. The fluent API reads like English:

```csharp
new CommandSaga<MoveCommand>()
    .WhenIn(PlayerTurn)                        // state guard → CommandRejectedEvent on failure
    .Validate(PlayerExists)                    // predicate  → CommandRejectedEvent on failure
    .Execute(RollDiceAndMove)                  // mutates GameState, can write to SagaContext
    .Emit(PlayerMoved)                         // reads state/context, appends an IEvent to the result
    .Emit(TileDamageEvent, when: ctx => ...)   // conditional emit — only appended when predicate is true
    .TransitionTo(ctx => ctx.Get<GameStateType>("nextState"))  // explicit state transition
```

**Branch step** — for cases where post-execution steps diverge structurally based on an outcome:

```csharp
.Branch(ctx => ctx.Get<MoveOutcome>())
    .Case(MoveOutcome.BlockedByEnemy,
        saga => saga.Execute(ApplyEnemyDamage)
                    .Emit(EnemyBlockedEvent)
                    .TransitionTo(PlayerTurnStarting))
    .Case(MoveOutcome.LandedOnShop,
        saga => saga.Emit(PlayerMovedEvent)
                    .TransitionTo(Shopping))
    .Default(
        saga => saga.Emit(PlayerMovedEvent)
                    .TransitionTo(PlayerTurnStarting))
```

- Steps execute in registration order — multiple `.Execute` and `.Emit` calls are allowed and ordered
- Guards (`.WhenIn`, `.Validate`) short-circuit: on failure they return a `CommandRejectedEvent` immediately and no further steps run
- `SagaContext` is a per-execution typed key-value bag (`ctx.Set<T>` / `ctx.Get<T>`) for passing data between steps (e.g. a dice roll computed in `.Execute` and read in `.Emit`)
- `[CallerArgumentExpression]` captures the method name passed to `.Validate` as the rejection reason automatically — no string needed
- `.TransitionTo(...)` fires **after the saga fully completes** — never mid-execution; state is only changed once all steps have run
- Branch cases are mini linear sagas and can themselves contain branches, but nesting is a code-smell — prefer flat branches + context-driven `TransitionTo`

### Adding a new command

1. Create `Commands/XxxCommand.cs` implementing `ICommand`
2. Add `[JsonDerivedType(typeof(XxxCommand), nameof(XxxCommand))]` to `ICommand` (skip for internal commands like `VillainTurnCommand`)
3. Create `GameEngine/Sagas/XxxCommandSaga.cs` with a static `Saga` property
4. Register it in the `Sagas` dictionary in `GameStateMachine`
5. Add a test class `Tests/GameEngine/XxxCommandTests.cs`
6. Add or update the command entry in `API.md` — include purpose, required state, and all fields

### Endpoint conventions

- Request and response records live at the bottom of `GameEndpoints.cs`, named `XxxRequest` / `XxxResponse`
- Endpoints that return data **must** use a typed response DTO — never return raw state objects or anonymous types
- Only include fields the client actually needs at that point in the flow (e.g. don't return board/health/tile data until the game starts)

### Testing conventions

- **Naming:** `<ClassUnderTest>_On<Command>_<WhatIsBeingTested>` — e.g. `GameStateMachine_OnMoveCommand_MovesPlayerAndAdvancesToNextPlayer`
- **Scope:** Prefer broad tests that cover the complete flow end-to-end (events emitted, state mutations, turn advancement). Narrow single-assertion tests are acceptable only for specific edge cases.
- **Structure:** Always use `// Arrange`, `// Act`, `// Assert` comments.
- **Player-action saga guard pattern:** Every player-action saga must include `.WhenIn(PlayerTurn).Validate(PlayerExists).Validate(IsActivePlayer)` — in that order. Tests must cover the wrong-state and wrong-player rejection cases.

### Adding a new event

1. Create `Events/XxxEvent.cs` implementing `IEvent`
2. Produce it from an `.Emit(...)` step in the relevant saga — no other wiring needed
3. Add or update the event entry in `API.md` — include purpose and all fields

### Key components

- **`GameEngine/Sagas/`** — one file per command; each owns its saga definition and all named step methods
- **`GameEngine/Saga/CommandSaga.cs`** — fluent saga builder; step types: `GuardStep`, `MutateStep`, `EmitStep`, `TransitionStep`, `BranchStep`
- **`GameEngine/Saga/SagaContext.cs`** — per-execution data bag threaded through all steps
- **`GameEngine/GameStateMachine.cs`** — two registries: `Type → ICommandSaga` for command dispatch, and `GameStateType → ICommand` for on-enter triggers; after every saga, if state changed and the new state has an on-enter command registered, it is auto-enqueued
- **`GameEngine/GameInstance.cs`** — per-game actor; owns the command channel, `ProcessLoop`, and `BroadcastAsync` (serializes each event as JSON and pushes to the SignalR group)
- **`GameEngine/GameState.cs`** — mutable state: player list, board, `TurnMarker`, `GameStateType`, `ShopInventory`
- **`Managers/GameInstanceManager.cs`** — DI singleton; factory and registry for `GameInstance` objects
- **`Endpoints/GameEndpoints.cs`** — minimal API endpoint definitions; one `Map(IEndpointRouteBuilder)` per domain area
- **`Hubs/EventHub.cs`** — SignalR hub; clients call `JoinGame`/`LeaveGame` to subscribe to a game group
- **`Constants/BoardConstants.cs`** — 36-tile board definition

### Command endpoint

```
POST /games/{gameId}/commands
Content-Type: application/json

{ "$type": "MoveCommand", "playerId": "..." }
```

Returns `202 Accepted`. Results arrive asynchronously via SignalR — the method name on the client matches the event type name (e.g. `MoveEvent`, `CommandRejectedEvent`).

### Meta-actions vs game commands

- **Game commands** (anything that mutates `GameState`, e.g. move, attack): go through the state machine / saga pipeline
- **Infrastructure actions** (create game, anything that doesn't change `GameState`): handled directly in HTTP endpoints via `GameInstanceManager`
- **Actions that change game state at lobby level** (join game, start game): go through the state machine — they are game rules
- **Chat and other orthogonal concerns**: do not go through the state machine

### Game state types

```
Initial → PlayerTurnStarting → PlayerTurn → PlayerActionEnding → PlayerTurn  (actions remain)
                             ↑             ↕ Shopping           → PlayerActionEnding → PlayerTurnStarting (turn over)
                             |             ↕ TreasureRoom                            → VillainTurn
                             └──────────────────────────────────────────────────────┘
                             → VillainTurn → PlayerTurnStarting → ...
                                           → Final
```

- **`Initial`** — lobby; players joining, game not yet started
- **`PlayerTurnStarting`** — on-enter emits `TurnChangedEvent` (the **sole** place `TurnChangedEvent` is emitted), transitions to `PlayerTurn`; `TurnMarker` is already set to the correct player before entering this state
- **`PlayerTurn`** — active player can issue actions (move, use card, etc.); all player-action sagas transition to `PlayerActionEnding` when done
- **`PlayerActionEnding`** — on-enter decrements `ActionsRemaining`; if actions remain → `PlayerTurn`; if exhausted → calls `TurnHelper.AdvanceToNextPlayerOrVillain` → `PlayerTurnStarting` (next player) or `VillainTurn`
- **`Shopping`** — active player landed on a shop tile; on-enter populates `GameState.ShopInventory` (3 cards at player level + 1 at level+1) and emits `ShopOpenedEvent`; player can issue `BuyCardCommand` multiple times, exits via `DoneShoppingCommand` which transitions to `PlayerActionEnding`
- **`TreasureRoom`** — active player selects a free reward card; resolved by `SelectRewardCommand` or `SkipChoiceCommand`, both transition to `PlayerActionEnding`
- **`VillainTurn`** — villain acts; driven by auto-enqueued `OnEnterCommand`
- **`Final`** — game over

Tile states (`Shopping`, `TreasureRoom`, etc.) are entered via `.TransitionTo(...)` in `MoveCommandSaga`. Simple tile effects (damage, heal) that need no player input are handled directly in `MoveCommandSaga` with Execute + Emit steps — no new state needed.

### Turn tracking

`GameState` holds a `TurnMarker`:

```csharp
public class TurnMarker
{
    public Guid ActivePlayerId { get; set; }
    public int ActivePlayerIndex { get; set; }
    public int ActionsRemaining { get; set; }
}
```

`Player.ActionsPerTurn` stores the player's base action budget (default 3, modifiable by effects). `TurnMarker.ActionsRemaining` is set to the active player's budget whenever the cursor advances — either in `StartGameCommandSaga` (first turn) or via `TurnHelper.AdvanceToNextPlayerOrVillain` (subsequent turns, called from `PlayerActionEndingOnEnterSaga`). Decrementing happens exclusively in `PlayerActionEndingOnEnterSaga` — sagas must **not** decrement actions themselves.

### On-enter triggers

When `GameStateMachine` applies a `.TransitionTo(...)` from a saga, it checks `OnEnterSagas` (a `Dictionary<GameStateType, ICommandSaga>` on `GameStateMachine`). If the new state has an entry, a shared `OnEnterCommand` is auto-enqueued. On the next loop iteration, `Handle` routes it to the correct saga by looking up `OnEnterSagas[currentState]`.

**No per-state command class is needed.** Register on-enter sagas directly in `GameStateMachine.OnEnterSagas`. Currently registered:

| State | File | What it does |
|---|---|---|
| `PlayerTurnStarting` | `PlayerTurnStartingOnEnterSaga.cs` | Emits `TurnChangedEvent`, transitions to `PlayerTurn` |
| `PlayerActionEnding` | `PlayerActionEndingOnEnterSaga.cs` | Decrements actions; routes to `PlayerTurn` or advances turn |
| `Shopping` | `ShoppingOnEnterSaga.cs` | Populates `ShopInventory`, emits `ShopOpenedEvent` |

- No `.WhenIn(...)` guard needed — the dispatch already guarantees the correct state is active
- If a state has no entry in `OnEnterSagas`, transitioning to it enqueues nothing
- On-enter sagas follow the same step API as command sagas (`Execute`, `Emit`, `TransitionTo`, `Branch`)

### Card system

Cards are defined in `Catalogs/Cards/CardCatalog.cs` (aggregates level catalogs `Level1CardCatalog` through `Level5CardCatalog`). Each `CardDefinition` has `Id`, `Name`, `Description`, `Price`, `Level`, `Usage` (`Active`/`Passive`), `TargetType`, and `Effect` (a delegate — not serializable, never put in events). Players have a `Level` property (default 1) used for shop card selection.

`GameState.ShopInventory` (`List<string>`) holds the card IDs available in the current shop visit. It is populated by `ShoppingOnEnterSaga` and cleared by `DoneShoppingCommandSaga`.

**Shopping flow (implemented):**

- `ShopOpenedEvent` — carries `List<ShopCardInfo>` (id, name, description, price — no logic/delegates); emitted by `ShoppingOnEnterSaga`
- `BuyCardCommand` — validates active player, card is in `ShopInventory`, player can afford it; deducts gold, adds to `Player.Inventory`, emits `CardAcquiredEvent`; stays in `Shopping`
- `DoneShoppingCommand` — clears `ShopInventory`, transitions to `PlayerActionEnding`

**Planned:**

- `SelectRewardCommand` — pick a free quest reward card; transitions to `PlayerActionEnding`
- `SkipChoiceCommand` — decline a treasure/reward; transitions to `PlayerActionEnding`
- `UseCardCommand` — use an active card during `PlayerTurn`; does not cost an action; emits `CardUsedEvent`
- Passive card application — sagas call an applicator at the right moment (e.g. `MoveCommandSaga` for on-move passives)

### Notable gaps (work in progress)

- `VillainTurn` has no on-enter saga — `VillainTurnCommand` must still be sent manually by the client
- `TreasureRoom` state has no on-enter saga and no commands (`SelectRewardCommand`, `SkipChoiceCommand`) — not yet implemented
- `UseCardCommand` not yet implemented
- Passive card application not yet wired into sagas
- No strongly-typed SignalR hub client interface yet (`Hub<T>`) — planned for when non-event push messages are needed
