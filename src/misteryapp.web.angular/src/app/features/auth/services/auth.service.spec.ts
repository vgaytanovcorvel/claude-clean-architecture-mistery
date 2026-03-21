import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { AuthMockApiService } from '../api/auth-mock-api.service';
import { AuthStateService } from '../../../core/auth/auth-state.service';
import { User } from '../../../domain/user.model';

describe('AuthService', () => {
  let service: AuthService;
  let mockApi: jasmine.SpyObj<AuthMockApiService>;
  let authState: jasmine.SpyObj<AuthStateService>;

  const fakeUser: User = {
    id: 'user-1',
    email: 'alice@example.com',
    name: 'Alice',
    avatarColor: '#7c3aed',
    createdAt: '2024-01-01T00:00:00.000Z',
  };

  beforeEach(() => {
    mockApi = jasmine.createSpyObj('AuthMockApiService', [
      'userValidateCredentials',
      'userSingleOrDefaultByEmail',
      'userCreate',
    ]);
    authState = jasmine.createSpyObj('AuthStateService', [
      'setSession',
      'clearSession',
    ]);

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: AuthMockApiService, useValue: mockApi },
        { provide: AuthStateService, useValue: authState },
      ],
    });

    service = TestBed.inject(AuthService);
  });

  describe('login', () => {
    it('should return failure when email is empty', () => {
      // Act
      const result = service.login({ email: '', password: 'pass123' });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('required');
    });

    it('should return failure when email format is invalid', () => {
      // Act
      const result = service.login({ email: 'not-an-email', password: 'pass123' });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('valid email');
    });

    it('should return failure when credentials are invalid', () => {
      // Arrange
      mockApi.userValidateCredentials.and.returnValue(null);

      // Act
      const result = service.login({ email: 'alice@example.com', password: 'wrong' });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('Invalid');
    });

    it('should set session and return success when credentials are valid', () => {
      // Arrange
      mockApi.userValidateCredentials.and.returnValue(fakeUser);

      // Act
      const result = service.login({ email: 'alice@example.com', password: 'pass123' });

      // Assert
      expect(result.success).toBeTrue();
      expect(authState.setSession).toHaveBeenCalledOnceWith(jasmine.objectContaining({
        userId: 'user-1',
        email: 'alice@example.com',
      }));
    });
  });

  describe('signup', () => {
    it('should return failure when email already exists', () => {
      // Arrange
      mockApi.userSingleOrDefaultByEmail.and.returnValue({
        id: 'user-1',
        email: 'alice@example.com',
        name: 'Alice',
        avatarColor: '#7c3aed',
        createdAt: '',
        passwordHash: '',
      });

      // Act
      const result = service.signup({
        email: 'alice@example.com',
        name: 'Alice',
        password: 'pass123',
      });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('already exists');
    });

    it('should return failure when name is too short', () => {
      // Arrange
      mockApi.userSingleOrDefaultByEmail.and.returnValue(null);

      // Act
      const result = service.signup({
        email: 'alice@example.com',
        name: 'A',
        password: 'pass123',
      });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('2 characters');
    });

    it('should return failure when password is too short', () => {
      // Arrange
      mockApi.userSingleOrDefaultByEmail.and.returnValue(null);

      // Act
      const result = service.signup({
        email: 'alice@example.com',
        name: 'Alice',
        password: 'abc',
      });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('6 characters');
    });

    it('should create user, set session and return success when request is valid', () => {
      // Arrange
      mockApi.userSingleOrDefaultByEmail.and.returnValue(null);
      mockApi.userCreate.and.returnValue(fakeUser);

      // Act
      const result = service.signup({
        email: 'alice@example.com',
        name: 'Alice',
        password: 'pass123',
      });

      // Assert
      expect(result.success).toBeTrue();
      expect(mockApi.userCreate).toHaveBeenCalledTimes(1);
      expect(authState.setSession).toHaveBeenCalledOnceWith(jasmine.objectContaining({
        userId: 'user-1',
      }));
    });
  });

  describe('logout', () => {
    it('should clear the session', () => {
      // Act
      service.logout();

      // Assert
      expect(authState.clearSession).toHaveBeenCalledTimes(1);
    });
  });
});
