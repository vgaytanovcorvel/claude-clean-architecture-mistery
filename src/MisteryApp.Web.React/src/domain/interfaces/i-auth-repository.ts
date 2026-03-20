import type { User } from '../models/user'

export interface IAuthRepository {
  authGetCurrentUser(): User | null
  authSignUp(email: string, name: string, password: string): Promise<User>
  authSignIn(email: string, password: string): Promise<User | null>
  authSignOut(): void
}
