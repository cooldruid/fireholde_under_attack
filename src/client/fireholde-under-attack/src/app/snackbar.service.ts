import { Injectable, signal } from '@angular/core';

export interface SnackMessage {
  id: number;
  text: string;
}

@Injectable({ providedIn: 'root' })
export class SnackbarService {
  readonly messages = signal<SnackMessage[]>([]);
  private nextId = 0;

  show(text: string, duration = 4000): void {
    const id = this.nextId++;
    this.messages.update(msgs => [...msgs, { id, text }]);
    setTimeout(() => {
      this.messages.update(msgs => msgs.filter(m => m.id !== id));
    }, duration);
  }
}
