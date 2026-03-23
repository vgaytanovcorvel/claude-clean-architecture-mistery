import { TestBed } from '@angular/core/testing'
import { AuthService } from './auth.service'
import { UserApiService } from '../../repositories/users/user-api.service'
import { User } from '../../domain/models/user'

describe('AuthService', () => {
  let service: AuthService
  let userApiSpy: jasmine.SpyObj<UserApiService>

  const fakeUser: User = {
    userId: 1,
    username: 'alice',
    displayName: 'Alice',
    createdAt: '2024-01-01T00:00:00Z',
  }

  beforeEach(() => {
    userApiSpy = jasmine.createSpyObj<UserApiService>('UserApiService', ['login', 'getUserByUsername'])

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: UserApiService, useValue: userApiSpy },
      ],
    })

    service = TestBed.inject(AuthService)
    localStorage.clear()
  })

  afterEach(() => {
    localStorage.clear()
  })

  describe('login', () => {
    it('should return the user when the API call succeeds', async () => {
      // Arrange
      userApiSpy.login.and.resolveTo(fakeUser)

      // Act
      const result = await service.login('alice', 'Alice')

      // Assert
      expect(result).toEqual(fakeUser)
      expect(userApiSpy.login).toHaveBeenCalledOnceWith({ username: 'alice', displayName: 'Alice' })
    })

    it('should store the user in localStorage when the API call succeeds', async () => {
      // Arrange
      userApiSpy.login.and.resolveTo(fakeUser)

      // Act
      await service.login('alice', 'Alice')

      // Assert
      const stored = JSON.parse(localStorage.getItem('currentUser')!)
      expect(stored).toEqual(fakeUser)
    })
  })

  describe('logout', () => {
    it('should remove the user from localStorage when called', () => {
      // Arrange
      localStorage.setItem('currentUser', JSON.stringify(fakeUser))

      // Act
      service.logout()

      // Assert
      expect(localStorage.getItem('currentUser')).toBeNull()
    })
  })

  describe('getCurrentUser', () => {
    it('should return the user when a user is stored in localStorage', () => {
      // Arrange
      localStorage.setItem('currentUser', JSON.stringify(fakeUser))

      // Act
      const result = service.getCurrentUser()

      // Assert
      expect(result).toEqual(fakeUser)
    })

    it('should return null when no user is stored in localStorage', () => {
      // Arrange — localStorage is already clear

      // Act
      const result = service.getCurrentUser()

      // Assert
      expect(result).toBeNull()
    })
  })

  describe('isLoggedIn', () => {
    it('should return true when a user is stored in localStorage', () => {
      // Arrange
      localStorage.setItem('currentUser', JSON.stringify(fakeUser))

      // Act
      const result = service.isLoggedIn()

      // Assert
      expect(result).toBeTrue()
    })

    it('should return false when no user is stored in localStorage', () => {
      // Arrange — localStorage is already clear

      // Act
      const result = service.isLoggedIn()

      // Assert
      expect(result).toBeFalse()
    })
  })
})
