import { Component, OnDestroy, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { LobbyPlayer } from '../api.service';
import { GameHubService } from '../game-hub.service';
import { PlayerIdentityService } from '../player-identity.service';

@Component({
  selector: 'app-lobby',
  standalone: true,
  templateUrl: './lobby.component.html',
  styleUrl: './lobby.component.scss',
})
export class LobbyComponent implements OnDestroy {
  readonly gameId: string;
  readonly players = signal<LobbyPlayer[]>([]);

  private readonly hub = inject(GameHubService);
  private readonly sub: Subscription;

  constructor() {
    const state = inject(Router).getCurrentNavigation()?.extras.state;
    const identity = inject(PlayerIdentityService);

    this.gameId = state?.['gameId'] ?? '';
    this.players.set(state?.['players'] ?? []);

    this.sub = this.hub.on<string>('PlayerJoinedEvent').subscribe(event => {
      const obj = JSON.parse(event);
      this.players.update(current => [...current, { id: obj.PlayerId }]);
    });

    this.hub.connect(this.gameId, identity.playerId);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    this.hub.disconnect();
  }
}
