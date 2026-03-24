import type { User } from '../models/user'

export interface IUserRepository {
  userFindAll(): Promise<readonly User[]>
}
