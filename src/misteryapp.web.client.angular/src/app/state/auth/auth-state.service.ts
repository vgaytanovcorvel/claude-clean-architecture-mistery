import { Injectable, inject, signal, computed } from '@angular/core'
import { AuthService } from '../../services/auth/auth.service'
import { User } from '../../domain/models/user'

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private readonly authService = inject(AuthService)

  readonly currentUser = signal<User | null>(this.authService.getCurrentUser())
  readonly isLoggedIn = computed(() => this.currentUser() !== null)

  async login(username: string, displayName: string): Promise<void> {
    const user = await this.authService.login(username, displayName)
    this.currentUser.set(user)
  }

  logout(): void {
    this.authService.logout()
    this.currentUser.set(null)
  }
}
