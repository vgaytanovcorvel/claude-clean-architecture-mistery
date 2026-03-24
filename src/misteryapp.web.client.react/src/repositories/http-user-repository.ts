import type { IUserRepository } from '../domain/interfaces/i-user-repository'
import type { User } from '../domain/models/user'
import type { ApiClient } from '../core/api-client'

interface ApiResponse<T> {
  success: boolean
  data: T
  error: string | null
  statusCode: number
}

export class HttpUserRepository implements IUserRepository {
  constructor(private readonly http: ApiClient) {}

  async userFindAll(): Promise<readonly User[]> {
    const response = await this.http.get<ApiResponse<User[]>>('/api/users')
    return response.data
  }
}
