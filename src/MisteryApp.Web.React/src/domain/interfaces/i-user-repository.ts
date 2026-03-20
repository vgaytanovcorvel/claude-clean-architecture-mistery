import type { User } from '../models/user'

export interface IUserRepository {
  userSingleById(id: string): Promise<User>
  userSingleOrDefaultByEmail(email: string): Promise<User | null>
  userUpdate(id: string, changes: Partial<Pick<User, 'name' | 'bio'>>): Promise<User>
}
