import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ApiService, CreateGameResponse, JoinGameResponse } from '../api.service';
import { PlayerIdentityService } from '../player-identity.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  private readonly api = inject(ApiService);
  private readonly identity = inject(PlayerIdentityService);
  private readonly router = inject(Router);

  readonly joinDialogOpen = signal(false);
  inviteCode = '';

  hostGame(): void {
    this.api
      .post<CreateGameResponse>('/games/create', { playerId: this.identity.playerId })
      .subscribe(response => {
        this.router.navigate(['/lobby'], {
          state: { gameId: response.gameId, players: [{ id: response.playerId }] },
        });
      });
  }

  openJoinDialog(): void {
    this.inviteCode = '';
    this.joinDialogOpen.set(true);
  }

  closeJoinDialog(): void {
    this.joinDialogOpen.set(false);
  }

  joinGame(): void {
    this.api
      .post<JoinGameResponse>('/games/join', {
        gameId: this.inviteCode,
        playerId: this.identity.playerId,
      })
      .subscribe(response => {
        this.joinDialogOpen.set(false);
        const playerIds = response.playerIds.map(id => ({id}));

        if(!playerIds.find(x => x.id == this.identity.playerId)) {
          playerIds.push({id: this.identity.playerId});
        }

        this.router.navigate(['/lobby'], {
          state: {
            gameId: response.gameId,
            players: playerIds,
          },
        });
      });
  }
}
