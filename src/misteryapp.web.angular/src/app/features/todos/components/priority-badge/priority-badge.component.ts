import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Priority, PRIORITY_LABELS } from '../../../../domain/priority.model';

@Component({
  selector: 'app-priority-badge',
  templateUrl: './priority-badge.component.html',
  styleUrl: './priority-badge.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PriorityBadgeComponent {
  priority = input.required<Priority>();

  protected get label(): string {
    return PRIORITY_LABELS[this.priority()];
  }
}
