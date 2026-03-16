import { Component, inject } from '@angular/core';

import { PLAYER_COLORS } from '../../core/player/player-colors';
import { GameStateService } from '../../core/game-state/game-state.service';

const MAX_HEALTH = 50;

@Component({
  selector: 'app-party-frames',
  standalone: true,
  templateUrl: './party-frames.component.html',
  styleUrl: './party-frames.component.scss',
})
export class PartyFramesComponent {
  protected readonly gameState = inject(GameStateService);
  protected readonly colors = PLAYER_COLORS;
  protected readonly maxHealth = MAX_HEALTH;
}
