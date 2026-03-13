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

Fireholde Under Attack is a 1-4 player board game. It is a cooperative game where each player plays as a wizard. They aim to defeat a boss which has different mechanics every game. Each player has health and money as well as cards with items and powers they get during the gameplay. On the board there are different tiles such as shops, treasures, etc.

## Architecture

This is an Angular 17 standalone-component application. There is no NgModule — components use the standalone API and import dependencies directly.

**Routing:** `app.routes.ts` defines a single route `/game` → `GameComponent`. The app root (`app.component.html`) is just a `<router-outlet />`.

**Game component** (`src/app/game/`) is the core of the application:
- Manages a 36-tile board (1 start tile + 35 shop tiles) laid out as a grid
- Tracks player position (`currentTile`) and animates a wizard token using absolute positioning with CSS transforms
- `cacheTilePositions()` captures tile DOM positions for animation
- `animateTo()` moves the wizard token with async animation between cached positions
- Tile data type is defined in `src/app/game/data/tile.ts`

**Assets** live in `src/assets/` (not `public/assets/`):
- `src/assets/tiles/square.png` — tile image
- `src/assets/wizard.png` — player token

**Styles:** SCSS is used throughout. Component SCSS files are currently empty; `styles.scss` is the global stylesheet.

**TypeScript:** Strict mode is enabled including `noImplicitReturns` and strict templates. Target is ES2022.
