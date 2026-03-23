import { TestBed } from '@angular/core/testing'
import { AuthStateService } from './auth-state.service'
import { AuthService } from '../../services/auth/auth.service'
import { User } from '../../domain/models/user'

describe('AuthStateService', () => {
  let service: AuthStateService
  let authServiceSpy: jasmine.SpyObj<AuthService>

  const fakeUser: User = {
    userId: 1,
    username: 'alice',
    displayName: 'Alice',
    createdAt: '2024-01-01T00:00:00Z',
  }

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['login', 'logout', 'getCurrentUser', 'isLoggedIn'])
    authServiceSpy.getCurrentUser.and.returnValue(null)

    TestBed.configureTestingModule({
      providers: [
        AuthStateService,
        { provide: AuthService, useValue: authServiceSpy },
      ],
    })

    service = TestBed.inject(AuthStateService)
  })

  describe('currentUser', () => {
    it('should initialize to null when no user is logged in', () => {
      // Assert
      expect(service.currentUser()).toBeNull()
    })
  })

  describe('isLoggedIn', () => {
    it('should return false when no user is logged in', () => {
      // Assert
      expect(service.isLoggedIn()).toBeFalse()
    })
  })

  describe('login', () => {
    it('should set the current user when login succeeds', async () => {
      // Arrange
      authServiceSpy.login.and.resolveTo(fakeUser)

      // Act
      await service.login('alice', 'Alice')

      // Assert
      expect(service.currentUser()).toEqual(fakeUser)
      expect(service.isLoggedIn()).toBeTrue()
      expect(authServiceSpy.login).toHaveBeenCalledOnceWith('alice', 'Alice')
    })
  })

  describe('logout', () => {
    it('should clear the current user when called', async () => {
      // Arrange
      authServiceSpy.login.and.resolveTo(fakeUser)
      await service.login('alice', 'Alice')

      // Act
      service.logout()

      // Assert
      expect(service.currentUser()).toBeNull()
      expect(service.isLoggedIn()).toBeFalse()
      expect(authServiceSpy.logout).toHaveBeenCalledTimes(1)
    })
  })
})
