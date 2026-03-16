import { Injectable, OnDestroy } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';

import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class GameHubService implements OnDestroy {
  private connection: signalR.HubConnection | null = null;
  private readonly subjects = new Map<string, Subject<unknown>>();
  private readonly reconnectedSubject = new Subject<void>();

  /** Emits whenever the SignalR connection is re-established after a drop. */
  readonly reconnected$ = this.reconnectedSubject.asObservable();

  /** Returns an Observable for a named hub event. Safe to call before connect(). */
  on<T>(event: string): Observable<T> {
    if (!this.subjects.has(event)) {
      const subject = new Subject<unknown>();
      this.subjects.set(event, subject);
      this.connection?.on(event, (data: unknown) => subject.next(data));
    }
    return this.subjects.get(event)!.asObservable() as Observable<T>;
  }

  async connect(gameId: string, playerId: string): Promise<void> {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hub`)
      .withAutomaticReconnect()
      .build();

    // Wire up any subjects registered before connect() was called
    this.subjects.forEach((subject, event) => {
      this.connection!.on(event, (data: unknown) => subject.next(data));
    });

    this.connection.onreconnected(() => this.reconnectedSubject.next());

    await this.connection.start();
    await this.connection.invoke('JoinGame', gameId, playerId);
  }

  disconnect(): void {
    this.connection?.stop();
    this.connection = null;
  }

  ngOnDestroy(): void {
    this.disconnect();
  }
}
