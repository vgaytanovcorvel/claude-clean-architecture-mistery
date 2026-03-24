import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import { TodoCard } from './todo-card'
import type { TodoItem } from '../../../domain/models/todo-item'

const fakeTodo: TodoItem = {
  id: 1,
  title: 'Buy groceries',
  isComplete: false,
  userId: 1,
  createdAtUtc: '2026-01-01T00:00:00Z',
  completedAtUtc: null,
}

const completedTodo: TodoItem = {
  ...fakeTodo,
  id: 2,
  title: 'Walk the dog',
  isComplete: true,
  completedAtUtc: '2026-01-02T00:00:00Z',
}

describe('TodoCard', () => {
  it('should render the todo title when mounted', () => {
    // Arrange + Act
    render(<TodoCard todo={fakeTodo} onToggle={vi.fn()} onDelete={vi.fn()} onUpdate={vi.fn()} />)

    // Assert
    expect(screen.getByText('Buy groceries')).toBeInTheDocument()
  })

  it('should render the completed date when the todo is complete', () => {
    // Arrange + Act
    render(<TodoCard todo={completedTodo} onToggle={vi.fn()} onDelete={vi.fn()} onUpdate={vi.fn()} />)

    // Assert
    expect(screen.getByText(/Completed/)).toBeInTheDocument()
  })

  describe('when the toggle button is clicked', () => {
    it('should call onToggle with the todo id', () => {
      // Arrange
      const onToggle = vi.fn()
      render(<TodoCard todo={fakeTodo} onToggle={onToggle} onDelete={vi.fn()} onUpdate={vi.fn()} />)

      // Act
      fireEvent.click(screen.getByRole('button', { name: /mark "Buy groceries" complete/i }))

      // Assert
      expect(onToggle).toHaveBeenCalledOnce()
      expect(onToggle).toHaveBeenCalledWith(1)
    })
  })

  describe('when the delete button is clicked', () => {
    it('should call onDelete with the todo id', () => {
      // Arrange
      const onDelete = vi.fn()
      render(<TodoCard todo={fakeTodo} onToggle={vi.fn()} onDelete={onDelete} onUpdate={vi.fn()} />)

      // Act
      fireEvent.click(screen.getByRole('button', { name: /delete "Buy groceries"/i }))

      // Assert
      expect(onDelete).toHaveBeenCalledOnce()
      expect(onDelete).toHaveBeenCalledWith(1)
    })
  })

  describe('when the title is double-clicked', () => {
    it('should show an edit input with the current title', () => {
      // Arrange
      render(<TodoCard todo={fakeTodo} onToggle={vi.fn()} onDelete={vi.fn()} onUpdate={vi.fn()} />)

      // Act
      fireEvent.doubleClick(screen.getByText('Buy groceries'))

      // Assert
      expect(screen.getByRole('textbox', { name: /edit todo title/i })).toHaveValue('Buy groceries')
    })

    it('should call onUpdate when the edit is committed with a new title', () => {
      // Arrange
      const onUpdate = vi.fn()
      render(<TodoCard todo={fakeTodo} onToggle={vi.fn()} onDelete={vi.fn()} onUpdate={onUpdate} />)
      fireEvent.doubleClick(screen.getByText('Buy groceries'))
      const input = screen.getByRole('textbox', { name: /edit todo title/i })

      // Act
      fireEvent.change(input, { target: { value: 'Buy vegetables' } })
      fireEvent.keyDown(input, { key: 'Enter' })

      // Assert
      expect(onUpdate).toHaveBeenCalledOnce()
      expect(onUpdate).toHaveBeenCalledWith(1, 'Buy vegetables')
    })

    it('should not call onUpdate when Escape is pressed', () => {
      // Arrange
      const onUpdate = vi.fn()
      render(<TodoCard todo={fakeTodo} onToggle={vi.fn()} onDelete={vi.fn()} onUpdate={onUpdate} />)
      fireEvent.doubleClick(screen.getByText('Buy groceries'))
      const input = screen.getByRole('textbox', { name: /edit todo title/i })

      // Act
      fireEvent.change(input, { target: { value: 'Changed' } })
      fireEvent.keyDown(input, { key: 'Escape' })

      // Assert
      expect(onUpdate).not.toHaveBeenCalled()
    })

    it('should not enter edit mode when the todo is complete', () => {
      // Arrange
      render(<TodoCard todo={completedTodo} onToggle={vi.fn()} onDelete={vi.fn()} onUpdate={vi.fn()} />)

      // Act
      fireEvent.doubleClick(screen.getByText('Walk the dog'))

      // Assert
      expect(screen.queryByRole('textbox', { name: /edit todo title/i })).not.toBeInTheDocument()
    })
  })
})
