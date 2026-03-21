import { Injectable, signal, computed } from '@angular/core';
import { AuthSession } from '../../domain/auth.model';
import { MockStorageService } from '../mock/mock-storage.service';
import { inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private readonly storage = inject(MockStorageService);

  // Synchronously hydrated from localStorage before first route render
  readonly session = signal<AuthSession | null>(this.storage.getSession());
  readonly isAuthenticated = computed(() => this.session() !== null);

  setSession(session: AuthSession): void {
    this.storage.saveSession(session);
    this.session.set(session);
  }

  clearSession(): void {
    this.storage.clearSession();
    this.session.set(null);
  }
}
