import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'gridPosition',
  standalone: true,
})
export class GridPositionPipe implements PipeTransform {
  transform(tileId: number): Record<string, string | number> {
    const sideSize = 10;

    if (tileId < sideSize) {
      return { gridRow: sideSize, gridColumn: tileId % sideSize };
    }
    if (tileId < sideSize * 2 - 1) {
      return { gridRow: 2 * sideSize - tileId, gridColumn: sideSize };
    }
    if (tileId < sideSize * 3 - 2) {
      return { gridRow: 1, gridColumn: 3 * sideSize - tileId - 1 };
    }
    if (tileId < sideSize * 4 - 3) {
      return { gridRow: (tileId % (3 * sideSize - 2)) + 1, gridColumn: 1 };
    }

    return { display: 'none' };
  }
}
