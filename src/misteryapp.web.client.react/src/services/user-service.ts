import type { IUserRepository } from '../domain/interfaces/i-user-repository'
import type { IUserService } from '../domain/interfaces/i-user-service'
import type { User } from '../domain/models/user'

export class UserService implements IUserService {
  constructor(private readonly userRepo: IUserRepository) {}

  getUsers(): Promise<readonly User[]> {
    return this.userRepo.userFindAll()
  }
}
