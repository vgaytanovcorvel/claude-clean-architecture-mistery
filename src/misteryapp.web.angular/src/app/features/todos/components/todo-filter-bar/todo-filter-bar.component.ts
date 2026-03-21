import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { TodoFilter, PriorityFilter } from '../../../../domain/todo.model';
import { Priority, PRIORITIES, PRIORITY_LABELS } from '../../../../domain/priority.model';

@Component({
  selector: 'app-todo-filter-bar',
  templateUrl: './todo-filter-bar.component.html',
  styleUrl: './todo-filter-bar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoFilterBarComponent {
  statusFilter = input.required<TodoFilter>();
  priorityFilter = input.required<PriorityFilter>();
  totalCount = input<number>(0);
  completedCount = input<number>(0);

  statusChanged = output<TodoFilter>();
  priorityChanged = output<PriorityFilter>();

  protected readonly priorities = PRIORITIES;
  protected readonly priorityLabels = PRIORITY_LABELS;
  protected readonly statusOptions: { value: TodoFilter; label: string }[] = [
    { value: 'all', label: 'All' },
    { value: 'active', label: 'Active' },
    { value: 'completed', label: 'Completed' },
  ];
}
