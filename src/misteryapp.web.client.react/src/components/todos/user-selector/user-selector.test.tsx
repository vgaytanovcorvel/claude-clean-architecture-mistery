import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import { UserSelector } from './user-selector'
import type { User } from '../../../domain/models/user'

const fakeUsers: User[] = [
  { id: 1, name: 'Alice', avatarUrl: 'https://example.com/alice.png' },
  { id: 2, name: 'Bob', avatarUrl: 'https://example.com/bob.png' },
]

describe('UserSelector', () => {
  it('should render a button for each user when users are provided', () => {
    // Arrange + Act
    render(<UserSelector users={fakeUsers} selectedUserId={null} onSelect={vi.fn()} />)

    // Assert
    expect(screen.getByRole('button', { name: /Alice/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /Bob/i })).toBeInTheDocument()
  })

  it('should mark the selected user button as pressed when selectedUserId matches', () => {
    // Arrange + Act
    render(<UserSelector users={fakeUsers} selectedUserId={1} onSelect={vi.fn()} />)

    // Assert
    expect(screen.getByRole('button', { name: /Alice/i })).toHaveAttribute('aria-pressed', 'true')
    expect(screen.getByRole('button', { name: /Bob/i })).toHaveAttribute('aria-pressed', 'false')
  })

  it('should call onSelect with the user id when a user chip is clicked', () => {
    // Arrange
    const onSelect = vi.fn()
    render(<UserSelector users={fakeUsers} selectedUserId={null} onSelect={onSelect} />)

    // Act
    fireEvent.click(screen.getByRole('button', { name: /Bob/i }))

    // Assert
    expect(onSelect).toHaveBeenCalledOnce()
    expect(onSelect).toHaveBeenCalledWith(2)
  })
})
