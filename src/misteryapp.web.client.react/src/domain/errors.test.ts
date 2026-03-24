import { describe, it, expect } from 'vitest'
import { AppError, NotFoundException, ValidationError, UnauthorizedError } from './errors'

describe('AppError', () => {
  it('should set message and code when constructed', () => {
    // Arrange + Act
    const error = new AppError('something broke', 'CUSTOM_CODE')

    // Assert
    expect(error.message).toBe('something broke')
    expect(error.code).toBe('CUSTOM_CODE')
    expect(error.name).toBe('AppError')
    expect(error).toBeInstanceOf(Error)
  })
})

describe('NotFoundException', () => {
  it('should set code to NOT_FOUND when constructed', () => {
    // Arrange + Act
    const error = new NotFoundException('item not found')

    // Assert
    expect(error.message).toBe('item not found')
    expect(error.code).toBe('NOT_FOUND')
    expect(error).toBeInstanceOf(AppError)
    expect(error).toBeInstanceOf(Error)
  })
})

describe('ValidationError', () => {
  it('should set code to VALIDATION_ERROR when constructed', () => {
    // Arrange + Act
    const error = new ValidationError('invalid input')

    // Assert
    expect(error.message).toBe('invalid input')
    expect(error.code).toBe('VALIDATION_ERROR')
    expect(error).toBeInstanceOf(AppError)
  })
})

describe('UnauthorizedError', () => {
  it('should set code to UNAUTHORIZED when constructed', () => {
    // Arrange + Act
    const error = new UnauthorizedError('not allowed')

    // Assert
    expect(error.message).toBe('not allowed')
    expect(error.code).toBe('UNAUTHORIZED')
    expect(error).toBeInstanceOf(AppError)
  })
})
