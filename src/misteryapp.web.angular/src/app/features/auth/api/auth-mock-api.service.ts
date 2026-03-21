import { Injectable, inject } from '@angular/core';
import { MockStorageService, StoredUser } from '../../../core/mock/mock-storage.service';
import { User, AVATAR_COLORS } from '../../../domain/user.model';

@Injectable({ providedIn: 'root' })
export class AuthMockApiService {
  private readonly storage = inject(MockStorageService);

  userSingleOrDefaultByEmail(email: string): StoredUser | null {
    const users = this.storage.getUsers();
    return users.find((u) => u.email.toLowerCase() === email.toLowerCase()) ?? null;
  }

  userCreate(data: {
    email: string;
    name: string;
    password: string;
  }): User {
    const users = this.storage.getUsers();
    const newUser: StoredUser = {
      id: crypto.randomUUID(),
      email: data.email,
      name: data.name,
      avatarColor: AVATAR_COLORS[users.length % AVATAR_COLORS.length],
      createdAt: new Date().toISOString(),
      // NOTE: btoa encoding is NOT production-safe — for mock/demo only
      passwordHash: btoa(data.password),
    };
    this.storage.saveUsers([...users, newUser]);
    return this.toUser(newUser);
  }

  userValidateCredentials(email: string, password: string): User | null {
    const stored = this.userSingleOrDefaultByEmail(email);
    if (!stored) return null;
    // NOTE: btoa comparison is NOT production-safe — for mock/demo only
    if (stored.passwordHash !== btoa(password)) return null;
    return this.toUser(stored);
  }

  userSingleById(id: string): User | null {
    const users = this.storage.getUsers();
    const stored = users.find((u) => u.id === id) ?? null;
    return stored ? this.toUser(stored) : null;
  }

  userUpdate(id: string, data: { name: string }): User | null {
    const users = this.storage.getUsers();
    const idx = users.findIndex((u) => u.id === id);
    if (idx === -1) return null;
    const updated: StoredUser = { ...users[idx], name: data.name };
    const newUsers = [...users.slice(0, idx), updated, ...users.slice(idx + 1)];
    this.storage.saveUsers(newUsers);
    return this.toUser(updated);
  }

  private toUser(stored: StoredUser): User {
    return {
      id: stored.id,
      email: stored.email,
      name: stored.name,
      avatarColor: stored.avatarColor,
      createdAt: stored.createdAt,
    };
  }
}
