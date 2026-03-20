import type { IUserRepository } from '../domain/interfaces/i-user-repository'
import type { User } from '../domain/models/user'
import { Result } from '../domain/result'

export class ProfileService {
  constructor(private readonly userRepo: IUserRepository) {}

  async getProfile(userId: string): Promise<Result<User>> {
    const user = await this.userRepo.userSingleById(userId)
    return Result.ok(user)
  }

  async updateProfile(
    userId: string,
    changes: { name?: string; bio?: string },
  ): Promise<Result<User>> {
    if (changes.name !== undefined && !changes.name.trim()) {
      return Result.fail('Name cannot be empty')
    }
    if (changes.name && changes.name.trim().length > 100) {
      return Result.fail('Name is too long (max 100 characters)')
    }
    if (changes.bio && changes.bio.length > 500) {
      return Result.fail('Bio is too long (max 500 characters)')
    }
    const updated = await this.userRepo.userUpdate(userId, {
      ...(changes.name !== undefined ? { name: changes.name.trim() } : {}),
      ...(changes.bio !== undefined ? { bio: changes.bio } : {}),
    })
    return Result.ok(updated)
  }
}
