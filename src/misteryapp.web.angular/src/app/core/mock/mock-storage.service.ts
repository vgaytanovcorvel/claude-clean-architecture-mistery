import { Injectable } from '@angular/core';
import { AuthSession } from '../../domain/auth.model';
import { Todo } from '../../domain/todo.model';

export interface StoredUser {
  id: string;
  email: string;
  name: string;
  avatarColor: string;
  createdAt: string;
  // NOTE: btoa encoding is NOT production-safe — for mock/demo only
  passwordHash: string;
}

const KEYS = {
  USERS: 'ma_users',
  TODOS: 'ma_todos',
  SESSION: 'ma_session',
} as const;

@Injectable({ providedIn: 'root' })
export class MockStorageService {
  // ── Users ─────────────────────────────────────────────────────────────────

  getUsers(): StoredUser[] {
    return this.read<StoredUser[]>(KEYS.USERS) ?? [];
  }

  saveUsers(users: StoredUser[]): void {
    this.write(KEYS.USERS, users);
  }

  // ── Todos ─────────────────────────────────────────────────────────────────

  getTodos(): Todo[] {
    return this.read<Todo[]>(KEYS.TODOS) ?? [];
  }

  saveTodos(todos: Todo[]): void {
    this.write(KEYS.TODOS, todos);
  }

  // ── Session ───────────────────────────────────────────────────────────────

  getSession(): AuthSession | null {
    return this.read<AuthSession>(KEYS.SESSION);
  }

  saveSession(session: AuthSession): void {
    this.write(KEYS.SESSION, session);
  }

  clearSession(): void {
    localStorage.removeItem(KEYS.SESSION);
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  private read<T>(key: string): T | null {
    const raw = localStorage.getItem(key);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as T;
    } catch {
      return null;
    }
  }

  private write<T>(key: string, value: T): void {
    localStorage.setItem(key, JSON.stringify(value));
  }
}
