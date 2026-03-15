import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { GameHubService } from './game-hub.service';
import { SnackbarService } from './snackbar.service';
import { SnackbarComponent } from './snackbar/snackbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SnackbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  constructor() {
    const hub = inject(GameHubService);
    const snackbar = inject(SnackbarService);

    hub.on<string>('CommandRejectedEvent').subscribe(raw => {
      const event = JSON.parse(raw);
      snackbar.show(event.Reason);
    });
  }
}
