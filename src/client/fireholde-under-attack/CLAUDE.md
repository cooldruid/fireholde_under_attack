# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Development server (localhost:4200)
npm start

# Build for production
npm run build

# Build with watch mode
npm run watch

# Run all tests
npm test

# Angular CLI (scaffolding, etc.)
npx ng generate component <name>
```

To run a single test file, use `--include` with the Karma config or focus tests with `fdescribe`/`fit` in the spec file.

## Description

Fireholde Under Attack is a 1-4 player cooperative board game where each player plays as a wizard. Players move around a Monopoly-style board and must collectively defeat a boss whose mechanics vary each game. Each player has health and money, and acquires cards with items and powers during gameplay. The board has different tile types including shops, treasures, and a start tile.

The game has no authentication — players are identified by a persistent UUID stored in `localStorage` (generated on first visit via `crypto.randomUUID()`), managed by `PlayerIdentityService`.

## Visual Design

The aesthetic is **cozy pixel-art RPG fantasy** set in a small town. Key design decisions:

- **Background:** `grass.jpg` tiled (repeated, not stretched) across all pages via `body` in `styles.scss`
- **Font:** [Cinzel Decorative](https://fonts.google.com/specimen/Cinzel+Decorative) (loaded via Google Fonts in `index.html`) for titles; monospace for UI text
- **Buttons:** Global `button` style in `styles.scss` — warm amber wooden-plank look with a hard 4px box-shadow (no blur) for pixel-art depth, snaps on `:active` via `translate(4px, 4px)`. No smooth transitions. `<a>` elements styled as buttons must duplicate these styles manually since the global rule only targets `button`

## Architecture

Angular 17 standalone-component application. No NgModule — components use the standalone API and import dependencies directly.

**Routing** (`app.routes.ts`):
- `/` → `HomeComponent` — main menu
- `/game` → `GameComponent`

**`HomeComponent`** (`src/app/home/`): Main menu with the game title and three buttons (Host Game, Join Game, GitHub link).

**`GameComponent`** (`src/app/game/`):
- 36-tile board (1 start + 35 shop tiles) arranged in a Monopoly-style loop — bottom row left→right, right column bottom→top, top row right→left, left column top→bottom
- Board data lives in `data/board.ts` as a constant; tile type in `data/tile.ts`
- Player position and token transform are Angular **signals** (`signal()`)
- `cacheTilePositions()` reads tile DOM rects after view init; `animateTo()` moves the wizard token step-by-step with a 200ms delay per tile
- Grid positioning logic is encapsulated in `GridPositionPipe` (`grid-position.pipe.ts`) — applied in the template as `tile.id | gridPosition`
- `Player` interface in `data/player.ts`; `TilePosition` interface in `data/tile-position.ts`

**`PlayerIdentityService`** (`src/app/player-identity.service.ts`): Singleton that resolves a persistent player UUID from `localStorage` on construction. Inject and read `.playerId`.

**Assets** live in `src/assets/` (not `public/assets/`):
- `src/assets/tiles/square.png` — tile image
- `src/assets/wizard.png` — player token
- `src/assets/grass.jpg` — background texture

**TypeScript:** Strict mode enabled including `noImplicitReturns` and strict templates. Target is ES2022.
