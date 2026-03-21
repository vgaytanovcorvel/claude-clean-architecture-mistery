import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ProfileStateService } from '../../state/profile-state.service';
import { AvatarBadgeComponent } from '../../components/avatar-badge/avatar-badge.component';
import { ProfileFormComponent } from '../../components/profile-form/profile-form.component';
import { GlassCardComponent } from '../../../../shared/components/glass-card/glass-card.component';
import { GlassButtonComponent } from '../../../../shared/components/glass-button/glass-button.component';
import { UpdateProfileRequest } from '../../../../domain/user.model';

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    AvatarBadgeComponent,
    ProfileFormComponent,
    GlassCardComponent,
    GlassButtonComponent,
    DatePipe,
  ],
})
export class ProfilePageComponent {
  protected readonly state = inject(ProfileStateService);
  protected readonly saveError = signal<string | null>(null);

  protected handleSave(req: UpdateProfileRequest): void {
    this.saveError.set(null);
    const error = this.state.updateProfile(req);
    if (error) this.saveError.set(error);
  }
}
