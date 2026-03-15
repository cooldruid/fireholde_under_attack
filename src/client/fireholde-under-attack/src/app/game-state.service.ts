import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { ApiService, GameState } from './api.service';

export interface SequencedEvent {
  sequenceNumber: number;
  [key: string]: unknown;
}

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
