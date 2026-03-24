import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { HomePage } from './home-page'

describe('HomePage', () => {
  it('should render the heading when mounted', () => {
    // Arrange + Act
    render(<HomePage />)

    // Assert
    expect(screen.getByRole('heading', { name: 'MisteryApp' })).toBeInTheDocument()
  })

  it('should render the welcome message when mounted', () => {
    // Arrange + Act
    render(<HomePage />)

    // Assert
    expect(screen.getByText('Welcome to MisteryApp. Start building your features.')).toBeInTheDocument()
  })
})
