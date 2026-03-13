import { Injectable } from '@angular/core';

const STORAGE_KEY = 'player_id';

@Injectable({ providedIn: 'root' })
export class PlayerIdentityService {
  readonly playerId: string = this.resolvePlayerId();

  private resolvePlayerId(): string {
    const existing = localStorage.getItem(STORAGE_KEY);
    if (existing) return existing;

    const id = crypto.randomUUID();
    localStorage.setItem(STORAGE_KEY, id);
    return id;
  }
}
