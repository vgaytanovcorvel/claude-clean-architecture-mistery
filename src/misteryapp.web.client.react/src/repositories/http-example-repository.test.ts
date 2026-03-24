import { describe, it, expect, vi, beforeEach } from 'vitest'
import { HttpExampleRepository } from './http-example-repository'
import { NotFoundException } from '../domain/errors'
import type { ApiClient } from '../core/api-client'

const mockApiClient = {
  get: vi.fn(),
  getOrNull: vi.fn(),
  post: vi.fn(),
  put: vi.fn(),
  delete: vi.fn(),
} as unknown as ApiClient

describe('HttpExampleRepository', () => {
  let repository: HttpExampleRepository

  beforeEach(() => {
    vi.clearAllMocks()
    repository = new HttpExampleRepository(mockApiClient)
  })

  describe('exampleFindAll', () => {
    it('should return mapped domain examples when API returns data', async () => {
      // Arrange
      const dtos = [
        { id: 1, name: 'Example 1', description: 'Desc 1', isActive: true, createdAt: '2025-01-01T00:00:00Z' },
      ]
      vi.mocked(mockApiClient.get).mockResolvedValue(dtos)

      // Act
      const result = await repository.exampleFindAll()

      // Assert
      expect(result).toHaveLength(1)
      expect(result[0]!.id).toBe(1)
      expect(result[0]!.name).toBe('Example 1')
      expect(result[0]!.createdAt).toBeInstanceOf(Date)
      expect(mockApiClient.get).toHaveBeenCalledOnce()
      expect(mockApiClient.get).toHaveBeenCalledWith('/api/examples')
    })
  })

  describe('exampleSingleById', () => {
    it('should return the example when it exists', async () => {
      // Arrange
      const dto = { id: 1, name: 'Example 1', description: 'Desc', isActive: true, createdAt: '2025-01-01T00:00:00Z' }
      vi.mocked(mockApiClient.getOrNull).mockResolvedValue(dto)

      // Act
      const result = await repository.exampleSingleById(1)

      // Assert
      expect(result.id).toBe(1)
      expect(result.createdAt).toBeInstanceOf(Date)
    })

    it('should throw NotFoundException when the example does not exist', async () => {
      // Arrange
      vi.mocked(mockApiClient.getOrNull).mockResolvedValue(null)

      // Act & Assert
      await expect(repository.exampleSingleById(999)).rejects.toThrow(NotFoundException)
    })
  })

  describe('exampleSingleOrDefaultById', () => {
    it('should return the example when it exists', async () => {
      // Arrange
      const dto = { id: 1, name: 'Example 1', description: 'Desc', isActive: true, createdAt: '2025-01-01T00:00:00Z' }
      vi.mocked(mockApiClient.getOrNull).mockResolvedValue(dto)

      // Act
      const result = await repository.exampleSingleOrDefaultById(1)

      // Assert
      expect(result).not.toBeNull()
      expect(result!.id).toBe(1)
      expect(mockApiClient.getOrNull).toHaveBeenCalledWith('/api/examples/1')
    })

    it('should return null when the example does not exist', async () => {
      // Arrange
      vi.mocked(mockApiClient.getOrNull).mockResolvedValue(null)

      // Act
      const result = await repository.exampleSingleOrDefaultById(999)

      // Assert
      expect(result).toBeNull()
      expect(mockApiClient.getOrNull).toHaveBeenCalledWith('/api/examples/999')
    })
  })
})
