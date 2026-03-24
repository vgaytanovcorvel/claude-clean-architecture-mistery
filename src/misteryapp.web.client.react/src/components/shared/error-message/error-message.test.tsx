import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { ErrorMessage } from './error-message'

describe('ErrorMessage', () => {
  it('should render the error message text when mounted', () => {
    // Arrange + Act
    render(<ErrorMessage message="Something went wrong" />)

    // Assert
    expect(screen.getByText('Something went wrong')).toBeInTheDocument()
  })

  it('should render with alert role when mounted', () => {
    // Arrange + Act
    render(<ErrorMessage message="Oops" />)

    // Assert
    expect(screen.getByRole('alert')).toBeInTheDocument()
  })
})
