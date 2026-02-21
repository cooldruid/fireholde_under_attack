import { Component } from '@angular/core';
import { Tile } from './data/tile';

@Component({
  selector: 'app-game',
  standalone: true,
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent {
  board: Tile[] = [
    {id: 1, type: 'start'},
    {id: 2, type: 'shop'},
    {id: 3, type: 'shop'},
    {id: 4, type: 'shop'},
    {id: 5, type: 'shop'},
    {id: 6, type: 'shop'},
    {id: 7, type: 'shop'},
    {id: 8, type: 'shop'},
    {id: 9, type: 'shop'},
    {id: 10, type: 'shop'},
    {id: 11, type: 'shop'},
    {id: 12, type: 'shop'},
    {id: 13, type: 'shop'},
    {id: 14, type: 'shop'},
    {id: 15, type: 'shop'},
    {id: 16, type: 'shop'},
    {id: 17, type: 'shop'},
    {id: 18, type: 'shop'},
    {id: 19, type: 'shop'},
    {id: 20, type: 'shop'},
    {id: 20, type: 'shop'},
    {id: 21, type: 'shop'},
    {id: 22, type: 'shop'},
    {id: 23, type: 'shop'},
    {id: 24, type: 'shop'},
    {id: 25, type: 'shop'},
    {id: 26, type: 'shop'},
    {id: 27, type: 'shop'},
    {id: 28, type: 'shop'},
    {id: 29, type: 'shop'},
    {id: 30, type: 'shop'},
    {id: 30, type: 'shop'},
    {id: 31, type: 'shop'},
    {id: 32, type: 'shop'},
    {id: 33, type: 'shop'},
    {id: 34, type: 'shop'},
    {id: 35, type: 'shop'},
    {id: 36, type: 'shop'},
    {id: 37, type: 'shop'},
    {id: 38, type: 'shop'},
    {id: 39, type: 'shop'},
    {id: 40, type: 'shop'}
  ]

  getGridPosition(index: number) {
    const sideSize = 10;

    if (index < sideSize) {
      // bottom row
      return { gridRow: sideSize, gridColumn: index % sideSize};
    }
    if (index < sideSize * 2 - 1) {
      // right column
      return { gridRow: 2 * sideSize - index, gridColumn: sideSize };
    }
    if (index < sideSize * 3 - 2) {
      // top row
      return { gridRow: 1, gridColumn: 3 * sideSize - index - 1 };
    }
    if (index < sideSize * 4 - 3) {
      // left column
      return { gridRow: (index % (3 * sideSize - 2)) + 1, gridColumn: 1 };
    }

    return {display: 'none'};
  }
}

// 16 1/1
// 17 2/1
// 18 3/1
// 19 4/1
// 20 5/1