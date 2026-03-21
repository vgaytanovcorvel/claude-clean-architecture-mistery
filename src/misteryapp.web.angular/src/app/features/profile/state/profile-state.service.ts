import { Injectable, inject, signal, resource } from '@angular/core';
import { AuthStateService } from '../../../core/auth/auth-state.service';
import { ProfileService } from '../services/profile.service';
import { UpdateProfileRequest } from '../../../domain/user.model';

@Injectable({ providedIn: 'root' })
export class ProfileStateService {
  private readonly authState = inject(AuthStateService);
  private readonly profileService = inject(ProfileService);

  readonly profileResource = resource({
    params: () => {
      const userId = this.authState.session()?.userId;
      return userId ? { userId } : undefined;
    },
    loader: ({ params }) =>
      Promise.resolve(this.profileService.getProfile(params.userId)),
  });

  readonly isEditing = signal(false);

  startEditing(): void {
    this.isEditing.set(true);
  }

  cancelEditing(): void {
    this.isEditing.set(false);
  }

  updateProfile(req: UpdateProfileRequest): string | null {
    const userId = this.authState.session()?.userId;
    if (!userId) return 'Not authenticated.';

    const result = this.profileService.updateProfile(userId, req);
    if (!result.success) return result.error;

    this.profileResource.reload();
    this.isEditing.set(false);
    return null;
  }
}
