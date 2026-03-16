export const TILE_SIZE = 110;
const SIDE_SIZE = 10;
const HALF = TILE_SIZE / 2;       // 55
export const TOKEN_SIZE = 48;
const PAD = (HALF - TOKEN_SIZE) / 2; // 3.5

export const SLOT_OFFSETS = [
  { top: PAD,        left: PAD },
  { top: PAD,        left: HALF + PAD },
  { top: HALF + PAD, left: PAD },
  { top: HALF + PAD, left: HALF + PAD },
];

export function tilePixelPosition(tileId: number): { top: number; left: number } {
  const id = tileId + 1;
  let row: number;
  let col: number;

  if (id < SIDE_SIZE) {
    row = SIDE_SIZE;
    col = id % SIDE_SIZE;
  } else if (id < SIDE_SIZE * 2 - 1) {
    row = 2 * SIDE_SIZE - id;
    col = SIDE_SIZE;
  } else if (id < SIDE_SIZE * 3 - 2) {
    row = 1;
    col = 3 * SIDE_SIZE - id - 1;
  } else {
    row = (id % (3 * SIDE_SIZE - 2)) + 1;
    col = 1;
  }

  return { top: (row - 1) * TILE_SIZE, left: (col - 1) * TILE_SIZE };
}
