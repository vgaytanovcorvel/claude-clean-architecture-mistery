import { describe, it, expect, beforeEach } from 'vitest'
import { useUserStore } from './user-store'

describe('useUserStore', () => {
  beforeEach(() => {
    useUserStore.setState({ selectedUserId: null })
  })

  it('should have null selectedUserId as initial state', () => {
    // Arrange + Act
    const state = useUserStore.getState()

    // Assert
    expect(state.selectedUserId).toBeNull()
  })

  describe('selectUser', () => {
    it('should set selectedUserId when called with a user id', () => {
      // Arrange
      const userId = 5

      // Act
      useUserStore.getState().selectUser(userId)

      // Assert
      expect(useUserStore.getState().selectedUserId).toBe(userId)
    })

    it('should update selectedUserId when called again with a different id', () => {
      // Arrange
      useUserStore.getState().selectUser(1)

      // Act
      useUserStore.getState().selectUser(7)

      // Assert
      expect(useUserStore.getState().selectedUserId).toBe(7)
    })
  })
})
