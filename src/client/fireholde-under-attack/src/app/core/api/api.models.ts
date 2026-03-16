export interface LobbyPlayer {
  playerId: string;
  playerName: string;
  currentTile: number;
}

export interface GameState {
  players: LobbyPlayer[];
  ownerId: string;
  activePlayerId: string;
  state: number;
  board: { tiles: unknown[] };
  sequenceNumber: number;
}

export interface CreateGameResponse {
  gameId: string;
  playerId: string;
}

export interface JoinGameResponse {
  gameId: string;
  playerIds: string[];
}
