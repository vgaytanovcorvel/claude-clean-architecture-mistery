import { describe, it, expect, vi, beforeEach } from 'vitest'
import { ExampleService } from './example-service'
import type { IExampleRepository } from '../domain/interfaces/i-example-repository'
import type { Example } from '../domain/models/example'

const mockExampleRepository: IExampleRepository = {
  exampleFindAll: vi.fn(),
  exampleSingleById: vi.fn(),
  exampleSingleOrDefaultById: vi.fn(),
}

describe('ExampleService', () => {
  let exampleService: ExampleService

  beforeEach(() => {
    vi.clearAllMocks()
    exampleService = new ExampleService(mockExampleRepository)
  })

  describe('getExamples', () => {
    it('should return all examples when examples exist', async () => {
      // Arrange
      const expectedExamples: readonly Example[] = [
        { id: 1, name: 'Example 1', description: 'Desc 1', isActive: true, createdAt: new Date('2025-01-01') },
      ]
      vi.mocked(mockExampleRepository.exampleFindAll).mockResolvedValue(expectedExamples)

      // Act
      const result = await exampleService.getExamples()

      // Assert
      expect(result).toEqual(expectedExamples)
      expect(mockExampleRepository.exampleFindAll).toHaveBeenCalledOnce()
    })
  })

  describe('getExampleById', () => {
    it('should return the example when the example exists', async () => {
      // Arrange
      const expected: Example = { id: 1, name: 'Example 1', description: 'Desc', isActive: true, createdAt: new Date('2025-01-01') }
      vi.mocked(mockExampleRepository.exampleSingleOrDefaultById).mockResolvedValue(expected)

      // Act
      const result = await exampleService.getExampleById(1)

      // Assert
      expect(result).toEqual(expected)
      expect(mockExampleRepository.exampleSingleOrDefaultById).toHaveBeenCalledOnce()
      expect(mockExampleRepository.exampleSingleOrDefaultById).toHaveBeenCalledWith(1)
    })

    it('should return null when the example does not exist', async () => {
      // Arrange
      vi.mocked(mockExampleRepository.exampleSingleOrDefaultById).mockResolvedValue(null)

      // Act
      const result = await exampleService.getExampleById(999)

      // Assert
      expect(result).toBeNull()
      expect(mockExampleRepository.exampleSingleOrDefaultById).toHaveBeenCalledOnce()
    })
  })
})
