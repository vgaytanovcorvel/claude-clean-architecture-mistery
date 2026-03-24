import { describe, it, expect, vi, beforeEach } from 'vitest'
import { HttpTodoRepository } from './http-todo-repository'
import type { ApiClient } from '../core/api-client'

const mockApiClient = {
  get: vi.fn(),
  getOrNull: vi.fn(),
  post: vi.fn(),
  put: vi.fn(),
  delete: vi.fn(),
} as unknown as ApiClient

describe('HttpTodoRepository', () => {
  let repository: HttpTodoRepository

  beforeEach(() => {
    vi.clearAllMocks()
    repository = new HttpTodoRepository(mockApiClient)
  })

  describe('todoFindByUserId', () => {
    it('should return todos from the API response data', async () => {
      // Arrange
      const apiResponse = {
        success: true,
        data: [
          { id: 1, title: 'Buy groceries', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null },
        ],
        error: null,
        statusCode: 200,
      }
      vi.mocked(mockApiClient.get).mockResolvedValue(apiResponse)

      // Act
      const result = await repository.todoFindByUserId(1)

      // Assert
      expect(result).toEqual(apiResponse.data)
      expect(mockApiClient.get).toHaveBeenCalledOnce()
      expect(mockApiClient.get).toHaveBeenCalledWith('/api/todos?userId=1')
    })
  })

  describe('todoCreate', () => {
    it('should return the created todo from the API response data', async () => {
      // Arrange
      const request = { title: 'New todo', userId: 1 }
      const apiResponse = {
        success: true,
        data: { id: 10, title: 'New todo', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null },
        error: null,
        statusCode: 201,
      }
      vi.mocked(mockApiClient.post).mockResolvedValue(apiResponse)

      // Act
      const result = await repository.todoCreate(request)

      // Assert
      expect(result).toEqual(apiResponse.data)
      expect(mockApiClient.post).toHaveBeenCalledOnce()
      expect(mockApiClient.post).toHaveBeenCalledWith('/api/todos', request)
    })
  })

  describe('todoUpdate', () => {
    it('should return the updated todo from the API response data', async () => {
      // Arrange
      const request = { title: 'Updated title' }
      const apiResponse = {
        success: true,
        data: { id: 1, title: 'Updated title', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null },
        error: null,
        statusCode: 200,
      }
      vi.mocked(mockApiClient.put).mockResolvedValue(apiResponse)

      // Act
      const result = await repository.todoUpdate(1, request)

      // Assert
      expect(result).toEqual(apiResponse.data)
      expect(mockApiClient.put).toHaveBeenCalledOnce()
      expect(mockApiClient.put).toHaveBeenCalledWith('/api/todos/1', request)
    })
  })

  describe('todoToggle', () => {
    it('should return the toggled todo from the API response data', async () => {
      // Arrange
      const apiResponse = {
        success: true,
        data: { id: 1, title: 'Buy groceries', isComplete: true, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: '2025-06-16T10:00:00Z' },
        error: null,
        statusCode: 200,
      }
      vi.mocked(mockApiClient.put).mockResolvedValue(apiResponse)

      // Act
      const result = await repository.todoToggle(1)

      // Assert
      expect(result).toEqual(apiResponse.data)
      expect(mockApiClient.put).toHaveBeenCalledOnce()
      expect(mockApiClient.put).toHaveBeenCalledWith('/api/todos/1/toggle', {})
    })
  })

  describe('todoDelete', () => {
    it('should call delete on the API client', async () => {
      // Arrange
      vi.mocked(mockApiClient.delete).mockResolvedValue(undefined)

      // Act
      await repository.todoDelete(1)

      // Assert
      expect(mockApiClient.delete).toHaveBeenCalledOnce()
      expect(mockApiClient.delete).toHaveBeenCalledWith('/api/todos/1')
    })
  })
})
