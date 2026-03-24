import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import { TodoInput } from './todo-input'

describe('TodoInput', () => {
  it('should render the input field when mounted', () => {
    // Arrange + Act
    render(<TodoInput onAdd={vi.fn()} />)

    // Assert
    expect(screen.getByRole('textbox', { name: /new todo title/i })).toBeInTheDocument()
  })

  it('should call onAdd with the trimmed title when the form is submitted', () => {
    // Arrange
    const onAdd = vi.fn()
    render(<TodoInput onAdd={onAdd} />)
    const input = screen.getByRole('textbox', { name: /new todo title/i })

    // Act
    fireEvent.change(input, { target: { value: '  Buy groceries  ' } })
    fireEvent.submit(input.closest('form')!)

    // Assert
    expect(onAdd).toHaveBeenCalledOnce()
    expect(onAdd).toHaveBeenCalledWith('Buy groceries')
  })

  it('should clear the input after successful submission', () => {
    // Arrange
    render(<TodoInput onAdd={vi.fn()} />)
    const input = screen.getByRole('textbox', { name: /new todo title/i })

    // Act
    fireEvent.change(input, { target: { value: 'New task' } })
    fireEvent.submit(input.closest('form')!)

    // Assert
    expect(input).toHaveValue('')
  })

  it('should not call onAdd when the input is empty', () => {
    // Arrange
    const onAdd = vi.fn()
    render(<TodoInput onAdd={onAdd} />)

    // Act
    fireEvent.submit(screen.getByRole('textbox', { name: /new todo title/i }).closest('form')!)

    // Assert
    expect(onAdd).not.toHaveBeenCalled()
  })

  it('should disable the input when disabled is true', () => {
    // Arrange + Act
    render(<TodoInput onAdd={vi.fn()} disabled />)

    // Assert
    expect(screen.getByRole('textbox', { name: /new todo title/i })).toBeDisabled()
  })
})
