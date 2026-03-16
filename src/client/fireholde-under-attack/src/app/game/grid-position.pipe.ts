import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'gridPosition',
  standalone: true,
})
export class GridPositionPipe implements PipeTransform {
  transform(tileId: number): Record<string, string | number> {
    const id = tileId + 1;
    const sideSize = 10;

    if (id < sideSize) {
      return { gridRow: sideSize, gridColumn: id % sideSize };
    }
    if (id < sideSize * 2 - 1) {
      return { gridRow: 2 * sideSize - id, gridColumn: sideSize };
    }
    if (id < sideSize * 3 - 2) {
      return { gridRow: 1, gridColumn: 3 * sideSize - id - 1 };
    }
    if (id < sideSize * 4 - 3) {
      return { gridRow: (id % (3 * sideSize - 2)) + 1, gridColumn: 1 };
    }

    return { display: 'none' };
  }
}
