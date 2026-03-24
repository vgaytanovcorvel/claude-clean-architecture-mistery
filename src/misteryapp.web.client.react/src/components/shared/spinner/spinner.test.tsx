import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { Spinner } from './spinner'

describe('Spinner', () => {
  it('should render with status role when mounted', () => {
    // Arrange + Act
    render(<Spinner />)

    // Assert
    expect(screen.getByRole('status')).toBeInTheDocument()
  })

  it('should render accessible loading text when mounted', () => {
    // Arrange + Act
    render(<Spinner />)

    // Assert
    expect(screen.getByText('Loading...')).toBeInTheDocument()
  })
})
