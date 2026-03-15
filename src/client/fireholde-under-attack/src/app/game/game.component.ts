import { AfterViewInit, Component, ElementRef, OnDestroy, QueryList, ViewChildren, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { BOARD } from './data/board';
import { Player } from './data/player';
import { TilePosition } from './data/tile-position';
import { DiceComponent } from './dice/dice.component';
import { GridPositionPipe } from './grid-position.pipe';
import { GameHubService } from '../game-hub.service';
import { GameStateService } from '../game-state.service';
import { PlayerIdentityService } from '../player-identity.service';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [GridPositionPipe, DiceComponent],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss',
})
export class GameComponent implements AfterViewInit, OnDestroy {
  readonly board = BOARD;

  @ViewChildren('tile') tiles!: QueryList<ElementRef>;

  private tileCoordinates: TilePosition[] = [];
  readonly tokenTransform = signal('');
  readonly player = signal<Player>({ name: '', currentTile: 1 });

  private readonly hub = inject(GameHubService);
  readonly gameState = inject(GameStateService);

  private reconnectSub?: Subscription;

  constructor() {
    const nav = inject(Router).getCurrentNavigation();
    const identity = inject(PlayerIdentityService);
    const gameId: string =
      nav?.extras.state?.['gameId'] ?? localStorage.getItem('current_game_id') ?? '';

    this.gameState.init(gameId);
    this.hub.connect(gameId, identity.playerId);

    this.reconnectSub = this.hub.reconnected$.subscribe(() =>
      this.gameState.refreshState()
    );
  }

  ngAfterViewInit(): void {
    this.cacheTilePositions();
    this.animateTo(1);
  }

  ngOnDestroy(): void {
    this.reconnectSub?.unsubscribe();
    this.hub.disconnect();
  }

  private cacheTilePositions(): void {
    this.tileCoordinates = this.tiles.map(tile => {
      const rect = tile.nativeElement.getBoundingClientRect();
      return { x: rect.left, y: rect.top };
    });
  }

  async movePlayer(squaresToMove: number): Promise<void> {
    const totalTiles = this.tiles.length;
    let current = this.player().currentTile;

    const targetTile =
      current + squaresToMove <= totalTiles
        ? current + squaresToMove
        : squaresToMove - (totalTiles - current);

    while (current !== targetTile) {
      current = (current % totalTiles) + 1;
      await this.animateTo(current);
    }

    this.player.update(p => ({ ...p, currentTile: targetTile }));
  }

  private animateTo(index: number): Promise<void> {
    return new Promise(resolve => {
      const coords = this.tileCoordinates[index - 1];
      this.tokenTransform.set(`translate(${coords.x}px, ${coords.y}px)`);
      setTimeout(resolve, 200);
    });
  }
}
