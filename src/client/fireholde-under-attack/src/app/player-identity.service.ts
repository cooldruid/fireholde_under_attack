import { Injectable, signal } from '@angular/core';

import { generatePlayerName } from './player-names';

const ID_KEY = 'player_id';
const NAME_KEY = 'player_name';

@Injectable({ providedIn: 'root' })
export class PlayerIdentityService {
  readonly playerId: string = this.resolvePlayerId();
  readonly playerName = signal<string>(this.resolvePlayerName());

  setName(name: string): void {
    this.playerName.set(name);
    localStorage.setItem(NAME_KEY, name);
  }

  private resolvePlayerId(): string {
    const existing = localStorage.getItem(ID_KEY);
    if (existing) return existing;

    const id = crypto.randomUUID();
    localStorage.setItem(ID_KEY, id);
    return id;
  }

  private resolvePlayerName(): string {
    const existing = localStorage.getItem(NAME_KEY);
    if (existing) return existing;

    const name = generatePlayerName();
    localStorage.setItem(NAME_KEY, name);
    return name;
  }
}
