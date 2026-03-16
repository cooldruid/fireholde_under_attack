import { Tile } from './tile';

export const BOARD: Tile[] = [
  { id: 0, type: 'start' },
  ...Array.from({ length: 35 }, (_, i) => ({ id: i + 1, type: 'shop' as const })),
];
