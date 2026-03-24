import type { User } from '../models/user'

export interface IUserService {
  getUsers(): Promise<readonly User[]>
}
