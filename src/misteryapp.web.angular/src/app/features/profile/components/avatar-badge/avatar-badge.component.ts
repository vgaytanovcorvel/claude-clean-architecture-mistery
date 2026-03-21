import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-avatar-badge',
  templateUrl: './avatar-badge.component.html',
  styleUrl: './avatar-badge.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AvatarBadgeComponent {
  name = input.required<string>();
  avatarColor = input<string>('#7c3aed');
  size = input<'sm' | 'md' | 'lg'>('md');

  protected get initial(): string {
    return this.name()[0]?.toUpperCase() ?? '?';
  }
}
