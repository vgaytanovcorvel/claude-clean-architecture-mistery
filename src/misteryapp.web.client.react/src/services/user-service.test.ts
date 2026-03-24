import { describe, it, expect, vi, beforeEach } from 'vitest'
import { UserService } from './user-service'
import type { IUserRepository } from '../domain/interfaces/i-user-repository'
import type { User } from '../domain/models/user'

const mockUserRepository: IUserRepository = {
  userFindAll: vi.fn(),
}

describe('UserService', () => {
  let userService: UserService

  beforeEach(() => {
    vi.clearAllMocks()
    userService = new UserService(mockUserRepository)
  })

  describe('getUsers', () => {
    it('should return all users when users exist', async () => {
      // Arrange
      const expectedUsers: readonly User[] = [
        { id: 1, name: 'Alice', avatarUrl: 'https://example.com/alice.png' },
        { id: 2, name: 'Bob', avatarUrl: 'https://example.com/bob.png' },
      ]
      vi.mocked(mockUserRepository.userFindAll).mockResolvedValue(expectedUsers)

      // Act
      const result = await userService.getUsers()

      // Assert
      expect(result).toEqual(expectedUsers)
      expect(mockUserRepository.userFindAll).toHaveBeenCalledOnce()
    })

    it('should return empty array when no users exist', async () => {
      // Arrange
      vi.mocked(mockUserRepository.userFindAll).mockResolvedValue([])

      // Act
      const result = await userService.getUsers()

      // Assert
      expect(result).toEqual([])
      expect(mockUserRepository.userFindAll).toHaveBeenCalledOnce()
    })
  })
})
