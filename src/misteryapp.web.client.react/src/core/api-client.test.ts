import { describe, it, expect, vi, beforeEach } from 'vitest'
import { ApiClient } from './api-client'

const mockFetch = vi.fn()
vi.stubGlobal('fetch', mockFetch)

describe('ApiClient', () => {
  let apiClient: ApiClient

  beforeEach(() => {
    vi.clearAllMocks()
    apiClient = new ApiClient('http://localhost:5000')
  })

  describe('get', () => {
    it('should return parsed JSON when the response is ok', async () => {
      // Arrange
      const data = { id: 1, name: 'Alice' }
      mockFetch.mockResolvedValue({
        ok: true,
        json: () => Promise.resolve(data),
      })

      // Act
      const result = await apiClient.get('/api/users')

      // Assert
      expect(result).toEqual(data)
      expect(mockFetch).toHaveBeenCalledOnce()
      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/users')
    })

    it('should throw AppError when the response is not ok', async () => {
      // Arrange
      mockFetch.mockResolvedValue({ ok: false, status: 500 })

      // Act & Assert
      await expect(apiClient.get('/api/users')).rejects.toThrow('API error 500')
    })
  })

  describe('getOrNull', () => {
    it('should return parsed JSON when the response is ok', async () => {
      // Arrange
      const data = { id: 1, name: 'Alice' }
      mockFetch.mockResolvedValue({
        ok: true,
        status: 200,
        json: () => Promise.resolve(data),
      })

      // Act
      const result = await apiClient.getOrNull('/api/users/1')

      // Assert
      expect(result).toEqual(data)
    })

    it('should return null when the response is 404', async () => {
      // Arrange
      mockFetch.mockResolvedValue({ ok: false, status: 404 })

      // Act
      const result = await apiClient.getOrNull('/api/users/999')

      // Assert
      expect(result).toBeNull()
    })

    it('should throw AppError when the response is a non-404 error', async () => {
      // Arrange
      mockFetch.mockResolvedValue({ ok: false, status: 500 })

      // Act & Assert
      await expect(apiClient.getOrNull('/api/users/1')).rejects.toThrow('API error 500')
    })
  })

  describe('post', () => {
    it('should send POST with JSON body and return parsed response', async () => {
      // Arrange
      const requestBody = { title: 'New todo', userId: 1 }
      const responseData = { id: 10, title: 'New todo', userId: 1 }
      mockFetch.mockResolvedValue({
        ok: true,
        json: () => Promise.resolve(responseData),
      })

      // Act
      const result = await apiClient.post('/api/todos', requestBody)

      // Assert
      expect(result).toEqual(responseData)
      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/todos', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      })
    })

    it('should throw AppError when the response is not ok', async () => {
      // Arrange
      mockFetch.mockResolvedValue({ ok: false, status: 400 })

      // Act & Assert
      await expect(apiClient.post('/api/todos', {})).rejects.toThrow('API error 400')
    })
  })

  describe('put', () => {
    it('should send PUT with JSON body and return parsed response', async () => {
      // Arrange
      const requestBody = { title: 'Updated title' }
      const responseData = { id: 1, title: 'Updated title' }
      mockFetch.mockResolvedValue({
        ok: true,
        json: () => Promise.resolve(responseData),
      })

      // Act
      const result = await apiClient.put('/api/todos/1', requestBody)

      // Assert
      expect(result).toEqual(responseData)
      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/todos/1', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      })
    })
  })

  describe('delete', () => {
    it('should send DELETE request when called', async () => {
      // Arrange
      mockFetch.mockResolvedValue({ ok: true })

      // Act
      await apiClient.delete('/api/todos/1')

      // Assert
      expect(mockFetch).toHaveBeenCalledWith('http://localhost:5000/api/todos/1', {
        method: 'DELETE',
      })
    })

    it('should throw AppError when the response is not ok', async () => {
      // Arrange
      mockFetch.mockResolvedValue({ ok: false, status: 404 })

      // Act & Assert
      await expect(apiClient.delete('/api/todos/999')).rejects.toThrow('API error 404')
    })
  })
})
