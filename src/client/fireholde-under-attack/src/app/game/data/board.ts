import { Tile } from './tile';

export const BOARD: Tile[] = [
  { id: 1, type: 'start' },
  ...Array.from({ length: 35 }, (_, i) => ({ id: i + 2, type: 'shop' as const })),
];
