import { describe, it, expect } from 'vitest'
import { Result } from './result'

describe('Result', () => {
  describe('ok', () => {
    it('should return a success result with the given value', () => {
      // Arrange + Act
      const result = Result.ok(42)

      // Assert
      expect(result.success).toBe(true)
      if (result.success) {
        expect(result.value).toBe(42)
      }
    })

    it('should return a success result with a complex value', () => {
      // Arrange
      const user = { id: 1, name: 'Alice' }

      // Act
      const result = Result.ok(user)

      // Assert
      expect(result.success).toBe(true)
      if (result.success) {
        expect(result.value).toEqual(user)
      }
    })
  })

  describe('fail', () => {
    it('should return a failure result with the given error message', () => {
      // Arrange + Act
      const result = Result.fail<number>('something went wrong')

      // Assert
      expect(result.success).toBe(false)
      if (!result.success) {
        expect(result.error).toBe('something went wrong')
      }
    })
  })
})
