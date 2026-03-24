import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { TodoListView } from './todo-list-view'
import type { TodoItem } from '../../../domain/models/todo-item'

const pendingTodo: TodoItem = {
  id: 1,
  title: 'Buy milk',
  isComplete: false,
  userId: 1,
  createdAtUtc: '2026-01-01T00:00:00Z',
  completedAtUtc: null,
}

const completedTodo: TodoItem = {
  id: 2,
  title: 'Walk the dog',
  isComplete: true,
  userId: 1,
  createdAtUtc: '2026-01-01T00:00:00Z',
  completedAtUtc: '2026-01-02T00:00:00Z',
}

const defaultProps = {
  todos: [] as readonly TodoItem[],
  isLoading: false,
  isAddDisabled: false,
  onAdd: vi.fn(),
  onToggle: vi.fn(),
  onDelete: vi.fn(),
  onUpdate: vi.fn(),
}

describe('TodoListView', () => {
  it('should render the spinner when isLoading is true', () => {
    // Arrange + Act
    render(<TodoListView {...defaultProps} isLoading />)

    // Assert
    expect(screen.getByRole('status')).toBeInTheDocument()
  })

  it('should render the error message when error is provided', () => {
    // Arrange + Act
    render(<TodoListView {...defaultProps} error="Network failure" />)

    // Assert
    expect(screen.getByText('Network failure')).toBeInTheDocument()
  })

  it('should render the empty state when todos is empty and not loading', () => {
    // Arrange + Act
    render(<TodoListView {...defaultProps} />)

    // Assert
    expect(screen.getByText('No todos yet')).toBeInTheDocument()
    expect(screen.getByText('Add one above to get started')).toBeInTheDocument()
  })

  it('should render pending todos when there are incomplete items', () => {
    // Arrange + Act
    render(<TodoListView {...defaultProps} todos={[pendingTodo]} />)

    // Assert
    expect(screen.getByText('Buy milk')).toBeInTheDocument()
  })

  it('should render the completed section with count when there are completed items', () => {
    // Arrange + Act
    render(<TodoListView {...defaultProps} todos={[pendingTodo, completedTodo]} />)

    // Assert
    expect(screen.getByText('Buy milk')).toBeInTheDocument()
    expect(screen.getByText('Walk the dog')).toBeInTheDocument()
    expect(screen.getByText('Completed (1)')).toBeInTheDocument()
  })

  it('should not render the completed section when there are no completed items', () => {
    // Arrange + Act
    render(<TodoListView {...defaultProps} todos={[pendingTodo]} />)

    // Assert
    expect(screen.queryByText(/^Completed/)).not.toBeInTheDocument()
  })
})
