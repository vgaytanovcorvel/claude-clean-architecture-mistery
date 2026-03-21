import { Injectable, inject } from '@angular/core';
import { AuthMockApiService } from '../../auth/api/auth-mock-api.service';
import { User, UpdateProfileRequest } from '../../../domain/user.model';

@Injectable({ providedIn: 'root' })
export class ProfileMockApiService {
  private readonly authApi = inject(AuthMockApiService);

  userSingleById(id: string): User | null {
    return this.authApi.userSingleById(id);
  }

  userUpdate(id: string, req: UpdateProfileRequest): User | null {
    return this.authApi.userUpdate(id, { name: req.name });
  }
}
