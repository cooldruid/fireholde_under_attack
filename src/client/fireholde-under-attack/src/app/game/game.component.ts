import { AfterViewInit, Component, ElementRef, QueryList, ViewChildren, signal } from '@angular/core';
import { BOARD } from './data/board';
import { Player } from './data/player';
import { TilePosition } from './data/tile-position';
import { GridPositionPipe } from './grid-position.pipe';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [GridPositionPipe],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss',
})
export class GameComponent implements AfterViewInit {
  readonly board = BOARD;

  @ViewChildren('tile') tiles!: QueryList<ElementRef>;

  private tileCoordinates: TilePosition[] = [];
  readonly tokenTransform = signal('');
  readonly player = signal<Player>({ currentTile: 1 });

  ngAfterViewInit(): void {
    this.cacheTilePositions();
    this.animateTo(1);
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
