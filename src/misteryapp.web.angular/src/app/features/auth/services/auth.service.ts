import { Injectable, inject } from '@angular/core';
import { AuthMockApiService } from '../api/auth-mock-api.service';
import { AuthStateService } from '../../../core/auth/auth-state.service';
import { LoginRequest, SignupRequest } from '../../../domain/auth.model';
import { Result } from '../../../domain/result.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(AuthMockApiService);
  private readonly authState = inject(AuthStateService);

  login(req: LoginRequest): Result<void> {
    const emailError = this.validateEmail(req.email);
    if (emailError) return Result.fail(emailError);
    if (!req.password) return Result.fail('Password is required.');

    const user = this.api.userValidateCredentials(req.email, req.password);
    if (!user) return Result.fail('Invalid email or password.');

    this.authState.setSession({
      userId: user.id,
      email: user.email,
      name: user.name,
      avatarColor: user.avatarColor,
    });
    return Result.ok(undefined);
  }

  signup(req: SignupRequest): Result<void> {
    const emailError = this.validateEmail(req.email);
    if (emailError) return Result.fail(emailError);

    const nameError = this.validateName(req.name);
    if (nameError) return Result.fail(nameError);

    const passwordError = this.validatePassword(req.password);
    if (passwordError) return Result.fail(passwordError);

    const existing = this.api.userSingleOrDefaultByEmail(req.email);
    if (existing) return Result.fail('An account with this email already exists.');

    const user = this.api.userCreate(req);
    this.authState.setSession({
      userId: user.id,
      email: user.email,
      name: user.name,
      avatarColor: user.avatarColor,
    });
    return Result.ok(undefined);
  }

  logout(): void {
    this.authState.clearSession();
  }

  private validateEmail(email: string): string | null {
    if (!email) return 'Email is required.';
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) return 'Please enter a valid email address.';
    return null;
  }

  private validateName(name: string): string | null {
    if (!name || name.trim().length === 0) return 'Name is required.';
    if (name.trim().length < 2) return 'Name must be at least 2 characters.';
    if (name.trim().length > 50) return 'Name must be 50 characters or fewer.';
    return null;
  }

  private validatePassword(password: string): string | null {
    if (!password) return 'Password is required.';
    if (password.length < 6) return 'Password must be at least 6 characters.';
    return null;
  }
}
