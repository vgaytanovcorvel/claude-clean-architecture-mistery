import { describe, it, expect, vi, beforeEach } from 'vitest'
import { HttpUserRepository } from './http-user-repository'
import type { ApiClient } from '../core/api-client'

const mockApiClient = {
  get: vi.fn(),
  getOrNull: vi.fn(),
  post: vi.fn(),
  put: vi.fn(),
  delete: vi.fn(),
} as unknown as ApiClient

describe('HttpUserRepository', () => {
  let repository: HttpUserRepository

  beforeEach(() => {
    vi.clearAllMocks()
    repository = new HttpUserRepository(mockApiClient)
  })

  describe('userFindAll', () => {
    it('should return users from the API response data', async () => {
      // Arrange
      const apiResponse = {
        success: true,
        data: [
          { id: 1, name: 'Alice', avatarUrl: 'https://example.com/alice.png' },
          { id: 2, name: 'Bob', avatarUrl: 'https://example.com/bob.png' },
        ],
        error: null,
        statusCode: 200,
      }
      vi.mocked(mockApiClient.get).mockResolvedValue(apiResponse)

      // Act
      const result = await repository.userFindAll()

      // Assert
      expect(result).toEqual(apiResponse.data)
      expect(mockApiClient.get).toHaveBeenCalledOnce()
      expect(mockApiClient.get).toHaveBeenCalledWith('/api/users')
    })

    it('should return empty array when no users exist', async () => {
      // Arrange
      const apiResponse = {
        success: true,
        data: [],
        error: null,
        statusCode: 200,
      }
      vi.mocked(mockApiClient.get).mockResolvedValue(apiResponse)

      // Act
      const result = await repository.userFindAll()

      // Assert
      expect(result).toEqual([])
      expect(mockApiClient.get).toHaveBeenCalledOnce()
    })
  })
})
