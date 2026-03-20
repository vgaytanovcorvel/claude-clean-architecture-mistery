import type { IAuthRepository } from '../domain/interfaces/i-auth-repository'
import type { IUserRepository } from '../domain/interfaces/i-user-repository'
import type { User } from '../domain/models/user'
import { Result } from '../domain/result'

function validateEmail(email: string): string | null {
  if (!email.trim()) return 'Email is required'
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return 'Invalid email address'
  return null
}

function validatePassword(password: string): string | null {
  if (!password) return 'Password is required'
  if (password.length < 6) return 'Password must be at least 6 characters'
  return null
}

export class AuthService {
  constructor(
    private readonly authRepo: IAuthRepository,
    private readonly userRepo: IUserRepository,
  ) {}

  getCurrentUser(): User | null {
    return this.authRepo.authGetCurrentUser()
  }

  async signUp(email: string, name: string, password: string): Promise<Result<User>> {
    const emailErr = validateEmail(email)
    if (emailErr) return Result.fail(emailErr)
    if (!name.trim()) return Result.fail('Name is required')
    const pwErr = validatePassword(password)
    if (pwErr) return Result.fail(pwErr)

    const existing = await this.userRepo.userSingleOrDefaultByEmail(email)
    if (existing) return Result.fail('An account with this email already exists')

    const user = await this.authRepo.authSignUp(email.trim(), name.trim(), password)
    return Result.ok(user)
  }

  async signIn(email: string, password: string): Promise<Result<User>> {
    const emailErr = validateEmail(email)
    if (emailErr) return Result.fail(emailErr)
    if (!password) return Result.fail('Password is required')

    const user = await this.authRepo.authSignIn(email.trim(), password)
    if (!user) return Result.fail('Invalid email or password')
    return Result.ok(user)
  }

  signOut(): void {
    this.authRepo.authSignOut()
  }
}
