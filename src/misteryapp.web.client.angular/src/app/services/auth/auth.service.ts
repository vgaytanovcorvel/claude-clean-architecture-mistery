import { Injectable, inject } from '@angular/core'
import { UserApiService } from '../../repositories/users/user-api.service'
import { User } from '../../domain/models/user'

const STORAGE_KEY = 'currentUser'

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly userApi = inject(UserApiService)

  async login(username: string, displayName: string): Promise<User> {
    const user = await this.userApi.login({ username, displayName })
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user))
    return user
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY)
  }

  getCurrentUser(): User | null {
    const json = localStorage.getItem(STORAGE_KEY)
    if (!json) return null
    return JSON.parse(json) as User
  }

  isLoggedIn(): boolean {
    return this.getCurrentUser() !== null
  }
}
