export interface LobbyPlayer {
  playerId: string;
  playerName: string;
  currentTile: number;
}

export interface TileState {
  id: number;
  type: string;
}

export interface GameState {
  players: LobbyPlayer[];
  ownerId: string;
  activePlayerId: string;
  state: number;
  board: TileState[];
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
