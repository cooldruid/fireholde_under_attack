import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ApiService, CreateGameResponse } from '../api.service';
import { PlayerIdentityService } from '../player-identity.service';
import { generatePlayerName } from '../player-names';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  private readonly api = inject(ApiService);
  readonly identity = inject(PlayerIdentityService);
  private readonly router = inject(Router);

  readonly joinDialogOpen = signal(false);
  inviteCode = '';

  rollName(): void {
    this.identity.setName(generatePlayerName());
  }

  hostGame(): void {
    this.api
      .post<CreateGameResponse>('/games/create', {
        playerId: this.identity.playerId,
        playerName: this.identity.playerName(),
      })
      .subscribe(response => {
        this.router.navigate(['/lobby'], { state: { gameId: response.gameId } });
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
      .post<{ gameId: string }>('/games/join', {
        gameId: this.inviteCode,
        playerId: this.identity.playerId,
        playerName: this.identity.playerName(),
      })
      .subscribe(response => {
        this.joinDialogOpen.set(false);
        this.router.navigate(['/lobby'], { state: { gameId: response.gameId } });
      });
  }
}
