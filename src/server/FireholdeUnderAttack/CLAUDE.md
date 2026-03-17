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
    .WhenIn(Playing)          // state guard → CommandRejectedEvent on failure
    .Validate(PlayerExists)   // predicate  → CommandRejectedEvent on failure
    .Execute(RollDiceAndMove) // mutates GameState, can write to SagaContext
    .Emit(PlayerMoved)        // reads state/context, appends an IEvent to the result
```

- Steps execute in registration order — multiple `.Execute` and `.Emit` calls are allowed and ordered
- Guards (`.WhenIn`, `.Validate`) short-circuit: on failure they return a `CommandRejectedEvent` immediately and no further steps run
- `SagaContext` is a per-execution typed key-value bag (`ctx.Set<T>` / `ctx.Get<T>`) for passing data between steps (e.g. a dice roll computed in `.Execute` and read in `.Emit`)
- `[CallerArgumentExpression]` captures the method name passed to `.Validate` as the rejection reason automatically — no string needed

### Adding a new command

1. Create `Commands/XxxCommand.cs` implementing `ICommand`
2. Add `[JsonDerivedType(typeof(XxxCommand), nameof(XxxCommand))]` to `ICommand` (skip for internal commands like `VillainTurnCommand`)
3. Create `GameEngine/Sagas/XxxCommandSaga.cs` with a static `Saga` property
4. Register it in the `Sagas` dictionary in `GameStateMachine`
5. Add a test class `Tests/GameEngine/XxxCommandTests.cs`

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

### Key components

- **`GameEngine/Sagas/`** — one file per command; each owns its saga definition and all named step methods
- **`GameEngine/Saga/CommandSaga.cs`** — fluent saga builder; step types: `GuardStep`, `MutateStep`, `EmitStep`
- **`GameEngine/Saga/SagaContext.cs`** — per-execution data bag threaded through all steps
- **`GameEngine/GameStateMachine.cs`** — static registry of `Type → ICommandSaga`; dispatches `Handle(ICommand)` to the right saga
- **`GameEngine/GameInstance.cs`** — per-game actor; owns the command channel, `ProcessLoop`, and `BroadcastAsync` (serializes each event as JSON and pushes to the SignalR group)
- **`GameEngine/GameState.cs`** — mutable state: player list, board, `GameStateType` (Initial → PlayerTurn ⇄ VillainTurn → Final)
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

### Card system (planned)

Players have an inventory of cards. Cards are defined as pure data in `Constants/CardCatalog.cs` and applied by a `Cards/CardEffectApplicator.cs`. There is no card logic in sagas — sagas call the applicator.

**Card definition shape:**

```csharp
record CardDefinition(string Id, string Name, int Cost, CardUsage Usage, CardEffect Effect, PassiveTrigger? Trigger);

// Effects and triggers are sealed record hierarchies — pure data, no logic
abstract record CardEffect;
record HealEffect(int Amount) : CardEffect;
// ...

abstract record PassiveTrigger;
record OnMoveTrigger : PassiveTrigger;
// ...
```

**Active cards** are played via `UseCardCommand` during a player's turn. Using a card does not consume the turn — the player still needs to move.

**Passive cards** are applied by sagas at the right moment (e.g. `MoveCommandSaga` calls `CardEffectApplicator.ApplyPassivesOnMove`). The trigger type on the definition controls when they fire.

#### Pending choices

When a player lands on a shop or fulfills a quest, the game needs to pause and wait for a selection before ending the turn. This is modelled as a nullable `PendingChoice` on `GameState` (no new `GameStateType` values needed):

```csharp
public record PendingChoice(PendingChoiceType Type, List<string> CardIds, IReadOnlySet<Type> AllowedCommands);
```

`AllowedCommands` is the set of command types that are permitted to resolve this choice (e.g. `BuyCardCommand`, `SkipChoiceCommand`). **The guard is enforced centrally in `GameStateMachine.Handle()`** — if a pending choice is active and the incoming command type is not in `AllowedCommands`, the state machine returns a `CommandRejectedEvent` immediately, before dispatching to any saga. No individual saga needs a `NoPendingChoice` validation step.

Sagas that create a pending choice (e.g. `MoveCommandSaga` on landing on a shop) set `state.PendingChoice` in an Execute step and conditionally skip turn advancement. Sagas that resolve a pending choice clear `state.PendingChoice` and then advance the turn.

**New commands:**

- `BuyCardCommand` — buy from shop; deducts gold, adds card to inventory, clears pending choice, advances turn
- `SelectRewardCommand` — pick a free quest reward card; same flow minus gold
- `SkipChoiceCommand` — decline a shop/reward; clears pending choice, advances turn
- `UseCardCommand` — use an active card during a player's turn (does not advance turn)

**New events:**

- `ChoicesAvailableEvent` — carries `PendingChoiceType` and list of `CardDefinition` for the client to display
- `CardAcquiredEvent` — card added to a player's inventory
- `CardUsedEvent` — active card was played

### Notable gaps (work in progress)

- Tile types in `BoardConstants` are empty strings
- `GameStateType` transitions (e.g. Initial → WaitingInLobby) are not enforced yet — no commands exist for lobby management
- No strongly-typed SignalR hub client interface yet (`Hub<T>`) — planned for when non-event push messages are needed
- Card system not yet implemented — see Card system section above
