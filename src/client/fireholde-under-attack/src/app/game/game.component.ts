import { Component, OnDestroy, ViewChild, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { BOARD } from './data/board';
import { SLOT_OFFSETS, tilePixelPosition } from './data/tile-math';
import { WIZARD_ASSETS } from '../core/player/player-colors';
import { DiceComponent } from './dice/dice.component';
import { PartyFramesComponent } from './party-frames/party-frames.component';
import { GridPositionPipe } from './grid-position.pipe';
import { GameHubService } from '../core/hub/game-hub.service';
import { GameStateService } from '../core/game-state/game-state.service';
import { PlayerIdentityService } from '../core/player/player-identity.service';
import { ApiService } from '../core/api/api.service';

interface MoveEvent {
  SequenceNumber: number;
  PlayerId: string;
  DiceRoll: number;
  NewTileId: number;
}

const delay = (ms: number): Promise<void> => new Promise(r => setTimeout(r, ms));

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [GridPositionPipe, DiceComponent, PartyFramesComponent],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss',
})
export class GameComponent implements OnDestroy {
  readonly board = BOARD;

  @ViewChild(DiceComponent) private dice?: DiceComponent;

  private readonly animatedTiles = signal<Map<string, number>>(new Map());
  private readonly localPlayerId: string;

  readonly showDice = signal(false);

  readonly tokenPositions = computed(() => {
    const players = this.gameState.state()?.players ?? [];
    const overrides = this.animatedTiles();
    return players.map((p, i) => {
      const tile = overrides.get(p.playerId) ?? p.currentTile;
      const { top, left } = tilePixelPosition(tile);
      const slot = SLOT_OFFSETS[i];
      return { playerId: p.playerId, name: p.playerName, top: top + slot.top, left: left + slot.left, asset: WIZARD_ASSETS[i] };
    });
  });

  private readonly api = inject(ApiService);
  private readonly hub = inject(GameHubService);
  readonly gameState = inject(GameStateService);
  private reconnectSub?: Subscription;
  private gameId = '';
  private eventQueue = Promise.resolve();

  readonly isMyTurn = computed(
    () => this.gameState.state()?.activePlayerId === this.localPlayerId
  );

  constructor() {
    const nav = inject(Router).getCurrentNavigation();
    const identity = inject(PlayerIdentityService);
    this.gameId =
      nav?.extras.state?.['gameId'] ?? localStorage.getItem('current_game_id') ?? '';

    this.localPlayerId = identity.playerId;
    this.gameState.init(this.gameId);
    this.hub.connect(this.gameId, identity.playerId);

    this.reconnectSub = this.hub.reconnected$.subscribe(() =>
      this.gameState.refreshState()
    );

    this.hub.on<string>('MoveEvent').subscribe(raw => {
      this.eventQueue = this.eventQueue.then(() => this.handleMoveEvent(raw));
    });

    this.hub.on<string>('TurnChangedEvent').subscribe(raw => {
      this.eventQueue = this.eventQueue.then(() => {
        const event = JSON.parse(raw) as { ActivePlayerId: string };
        this.gameState.updateActivePlayer(event.ActivePlayerId);
      });
    });
  }

  ngOnDestroy(): void {
    this.reconnectSub?.unsubscribe();
    this.hub.disconnect();
  }

  async movePlayer(squaresToMove: number): Promise<void> {
    const currentTile =
      this.animatedTiles().get(this.localPlayerId) ??
      this.gameState.state()?.players.find(p => p.playerId === this.localPlayerId)?.currentTile ??
      1;
    const totalTiles = this.board.length;
    const targetTile = (currentTile + squaresToMove) % totalTiles;

    await this.animatePlayerTo(this.localPlayerId, targetTile);
  }

  move(): void {
    this.api
      .post<void>(`/games/${this.gameId}/commands`, {
        $type: 'MoveCommand',
        playerId: this.localPlayerId,
      })
      .subscribe();
  }

  private async handleMoveEvent(raw: string): Promise<void> {
    const event = JSON.parse(raw) as MoveEvent;

    this.showDice.set(true);
    await delay(0); // let Angular render DiceComponent
    await this.dice!.roll(event.DiceRoll);

    await delay(100);
    await this.animatePlayerTo(event.PlayerId, event.NewTileId);

    this.gameState.updatePlayerTile(event.PlayerId, event.NewTileId);
    this.animatedTiles.update(m => { const next = new Map(m); next.delete(event.PlayerId); return next; });

    await delay(100);
    this.showDice.set(false);
  }

  private async animatePlayerTo(playerId: string, targetTile: number): Promise<void> {
    const totalTiles = this.board.length;
    let tile =
      this.animatedTiles().get(playerId) ??
      this.gameState.state()?.players.find(p => p.playerId === playerId)?.currentTile ??
      1;

    while (tile !== targetTile) {
      tile = (tile + 1) % totalTiles;
      this.animatedTiles.update(m => new Map(m).set(playerId, tile));
      await delay(200);
    }

  }
}
