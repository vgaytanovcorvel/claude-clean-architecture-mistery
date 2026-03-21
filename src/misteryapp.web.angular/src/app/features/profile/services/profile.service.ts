import { Injectable, inject } from '@angular/core';
import { ProfileMockApiService } from '../api/profile-mock-api.service';
import { AuthStateService } from '../../../core/auth/auth-state.service';
import { User, UpdateProfileRequest } from '../../../domain/user.model';
import { Result } from '../../../domain/result.model';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly api = inject(ProfileMockApiService);
  private readonly authState = inject(AuthStateService);

  getProfile(userId: string): User | null {
    return this.api.userSingleById(userId);
  }

  updateProfile(userId: string, req: UpdateProfileRequest): Result<User> {
    const name = req.name.trim();
    if (!name) return Result.fail('Name is required.');
    if (name.length < 2) return Result.fail('Name must be at least 2 characters.');
    if (name.length > 50) return Result.fail('Name must be 50 characters or fewer.');

    const updated = this.api.userUpdate(userId, { name });
    if (!updated) return Result.fail('User not found.');

    // Keep auth session in sync with updated name
    const session = this.authState.session();
    if (session) {
      this.authState.setSession({ ...session, name: updated.name });
    }

    return Result.ok(updated);
  }
}
