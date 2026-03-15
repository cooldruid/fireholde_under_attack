import { Component, inject } from '@angular/core';

import { SnackbarService } from '../snackbar.service';

@Component({
  selector: 'app-snackbar',
  standalone: true,
  templateUrl: './snackbar.component.html',
  styleUrl: './snackbar.component.scss',
})
export class SnackbarComponent {
  readonly snackbar = inject(SnackbarService);
}
