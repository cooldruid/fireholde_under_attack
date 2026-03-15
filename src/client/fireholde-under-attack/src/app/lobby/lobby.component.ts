import { Component, OnDestroy, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { ApiService, GameState, LobbyPlayer } from '../api.service';
import { GameHubService } from '../game-hub.service';
import { PlayerIdentityService } from '../player-identity.service';

const PLAYER_COLORS = ['#6699cc', '#66aa66', '#cc5555', '#f5d080'];

@Component({
  selector: 'app-lobby',
  standalone: true,
  templateUrl: './lobby.component.html',
  styleUrl: './lobby.component.scss',
})
export class LobbyComponent implements OnDestroy {
  readonly playerColors = PLAYER_COLORS;
  readonly gameId: string;
  readonly players = signal<LobbyPlayer[]>([]);
  readonly ownerId = signal<string>('');

  private readonly api = inject(ApiService);
  private readonly hub = inject(GameHubService);
  private readonly identity = inject(PlayerIdentityService);
  private readonly router = inject(Router);
  private readonly sub: Subscription;

  readonly isOwner: ReturnType<typeof computed<boolean>>;

  constructor() {
    const state = this.router.getCurrentNavigation()?.extras.state;

    this.gameId = state?.['gameId'] ?? '';
    this.isOwner = computed(() => this.ownerId() === this.identity.playerId);

    this.hub.on<string>('GameStartedEvent').subscribe(() => {
      localStorage.setItem('current_game_id', this.gameId);
      this.router.navigate(['/game'], { state: { gameId: this.gameId } });
    });

    this.sub = this.hub.on<string>('PlayerJoinedEvent').subscribe(event => {
      const obj = JSON.parse(event);
      this.players.update(current =>
        current.some(p => p.playerId === obj.PlayerId)
          ? current
          : [...current, { playerId: obj.PlayerId, playerName: obj.PlayerName ?? '' }]
      );
    });

    this.hub.connect(this.gameId, this.identity.playerId).then(() => {
      this.api.get<GameState>(`/games/${this.gameId}/state`).subscribe(state => {
        this.players.set(state.players);
        this.ownerId.set(state.ownerId);
      });
    });
  }

  startGame(): void {
    this.api
      .post<void>(`/games/${this.gameId}/commands`, {
        $type: 'StartGameCommand',
        playerId: this.identity.playerId,
      })
      .subscribe(() => {
        localStorage.setItem('current_game_id', this.gameId);
        this.router.navigate(['/game'], { state: { gameId: this.gameId } });
      });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    this.hub.disconnect();
  }
}
