import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { ApiService } from '../api/api.service';
import { GameState } from '../api/api.models';
import { SequencedEvent } from './game-state.models';

@Injectable({ providedIn: 'root' })
export class GameStateService {
  private readonly api = inject(ApiService);

  readonly state = signal<GameState | null>(null);
  private lastSequenceNumber = -1;
  private gameId = '';

  async init(gameId: string): Promise<void> {
    this.gameId = gameId;
    await this.refreshState();
  }

  async refreshState(): Promise<void> {
    const snapshot = await firstValueFrom(
      this.api.get<GameState>(`/games/${this.gameId}/state`)
    );
    this.lastSequenceNumber = snapshot.sequenceNumber;
    this.state.set(snapshot);
  }

  /**
   * Call for every incoming SignalR event (including CommandRejectedEvent).
   * If there's a sequence gap, fetches a fresh snapshot and returns false.
   * Otherwise increments the counter, calls apply(), and returns true.
   * CommandRejectedEvent: pass a no-op apply() — the counter still advances.
   */
  updateActivePlayer(playerId: string): void {
    this.state.update(s => s ? { ...s, activePlayerId: playerId } : s);
  }

  updatePlayerTile(playerId: string, tileId: number): void {
    this.state.update(s =>
      s ? { ...s, players: s.players.map(p => p.playerId === playerId ? { ...p, currentTile: tileId } : p) } : s
    );
  }

  async onEvent(event: SequencedEvent, apply: () => void): Promise<boolean> {
    if (event.sequenceNumber !== this.lastSequenceNumber + 1) {
      await this.refreshState();
      return false;
    }
    this.lastSequenceNumber = event.sequenceNumber;
    apply();
    return true;
  }
}
