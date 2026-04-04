# Fireholde Under Attack — Front-End API Reference

Real-time multiplayer game server. Commands are sent via HTTP; results arrive asynchronously over SignalR.

## Transport

### Sending commands
```
POST /games/{gameId}/commands
Content-Type: application/json

{ "$type": "<CommandName>", ...fields }
```
Returns `202 Accepted` immediately. The outcome arrives as a SignalR event.

### Receiving events
Connect to the SignalR hub at `/events`, then call `JoinGame(gameId)` to subscribe to a game's event stream. Events are pushed as JSON with the method name matching the event type (e.g. the server calls `MoveEvent` on the client with the event payload).

`SequenceNumber` on every event is a monotonically increasing integer per game — use it to detect missed or out-of-order messages.

---

## Commands

### `JoinGameCommand`
**State required:** `Initial` (lobby)  
**Purpose:** A new player joins the lobby before the game starts.

| Field | Type | Description |
|---|---|---|
| `$type` | `string` | `"JoinGameCommand"` |
| `playerId` | `uuid` | Unique ID for the joining player (client-generated) |
| `playerName` | `string` | Display name |

---

### `StartGameCommand`
**State required:** `Initial`  
**Purpose:** The game owner starts the game. Initialises the turn to the first player, sets round to 1, transitions to the first player's turn.

| Field | Type | Description |
|---|---|---|
| `$type` | `string` | `"StartGameCommand"` |

---

### `MoveCommand`
**State required:** `PlayerTurn`  
**Purpose:** The active player rolls the dice and moves. If they land on a shop tile the game enters `Shopping`; otherwise it goes to `PlayerActionEnding` to decrement their action count.

| Field | Type | Description |
|---|---|---|
| `$type` | `string` | `"MoveCommand"` |
| `playerId` | `uuid` | Must be the current active player |

---

### `BuyCardCommand`
**State required:** `Shopping`  
**Purpose:** The active player buys one card from the current shop. Gold is deducted and the card is added to their inventory. The player may issue this command multiple times to buy more than one card. The shop stays open until `DoneShoppingCommand` is sent.

| Field | Type | Description |
|---|---|---|
| `$type` | `string` | `"BuyCardCommand"` |
| `playerId` | `uuid` | Must be the current active player |
| `cardId` | `string` | ID of a card currently listed in the open shop |

---

### `DoneShoppingCommand`
**State required:** `Shopping`  
**Purpose:** The active player finishes shopping and closes the shop. Transitions to `PlayerActionEnding`, which will decrement their action count and either return to `PlayerTurn` or advance the turn.

| Field | Type | Description |
|---|---|---|
| `$type` | `string` | `"DoneShoppingCommand"` |
| `playerId` | `uuid` | Must be the current active player |

---

## Events

### `CommandRejectedEvent`
**Purpose:** The last command was invalid (wrong game state, wrong player, failed validation, etc.). The game state is unchanged. Display the reason to the player who sent the command.

| Field | Type | Description |
|---|---|---|
| `sequenceNumber` | `int` | |
| `gameId` | `uuid` | |
| `reason` | `string` | Human-readable rejection reason |

---

### `PlayerJoinedEvent`
**Purpose:** A new player successfully joined the lobby. Update the lobby player list.

| Field | Type | Description |
|---|---|---|
| `sequenceNumber` | `int` | |
| `gameId` | `uuid` | |
| `playerId` | `uuid` | |
| `playerName` | `string` | |

---

### `GameStartedEvent`
**Purpose:** The game has started. Transition from the lobby to the game board. The first player's turn is about to begin — a `TurnChangedEvent` will follow immediately.

| Field | Type | Description |
|---|---|---|
| `sequenceNumber` | `int` | |
| `gameId` | `uuid` | |
| `activePlayerId` | `uuid` | The first player to act |
| `round` | `int` | Always `1` at game start |

---

### `TurnChangedEvent`
**Purpose:** The active player has changed. Update the UI to highlight whose turn it is. Emitted at the start of every player's turn (including after villain turn). Not emitted mid-turn when the same player still has actions remaining.

| Field | Type | Description |
|---|---|---|
| `sequenceNumber` | `int` | |
| `gameId` | `uuid` | |
| `activePlayerId` | `uuid?` | The player whose turn it now is; `null` during villain turn |
| `isVillainTurn` | `bool` | `true` when the villain is acting |
| `round` | `int` | Current round number |

---

### `MoveEvent`
**Purpose:** A player moved. Update the player's position on the board. The dice roll is included for display purposes.

| Field | Type | Description |
|---|---|---|
| `sequenceNumber` | `int` | |
| `playerId` | `uuid` | The player who moved |
| `diceRoll` | `int` | The value rolled (1–6) |
| `newTileId` | `int` | The tile index the player landed on |

---

### `ShopOpenedEvent`
**Purpose:** The active player landed on a shop tile. Display the shop UI with the listed cards. The player can now send `BuyCardCommand` and `DoneShoppingCommand`.

| Field | Type | Description |
|---|---|---|
| `sequenceNumber` | `int` | |
| `gameId` | `uuid` | |
| `playerId` | `uuid` | The player who entered the shop |
| `availableCards` | `ShopCardInfo[]` | Cards available for purchase |

**`ShopCardInfo`**

| Field | Type | Description |
|---|---|---|
| `id` | `string` | Card identifier — use this in `BuyCardCommand.cardId` |
| `name` | `string` | Display name |
| `description` | `string` | Card effect description |
| `price` | `int` | Gold cost |
| `level` | `int` | Card tier (1–5) |

---

### `CardAcquiredEvent`
**Purpose:** A player successfully bought or received a card. Add it to their displayed inventory.

| Field | Type | Description |
|---|---|---|
| `sequenceNumber` | `int` | |
| `gameId` | `uuid` | |
| `playerId` | `uuid` | The player who received the card |
| `cardId` | `string` | ID of the acquired card |
