import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Todo } from '../../../../domain/todo.model';
import { PriorityBadgeComponent } from '../priority-badge/priority-badge.component';

@Component({
  selector: 'app-todo-card',
  templateUrl: './todo-card.component.html',
  styleUrl: './todo-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [PriorityBadgeComponent, DatePipe],
})
export class TodoCardComponent {
  todo = input.required<Todo>();

  toggleComplete = output<Todo>();
  editRequest = output<Todo>();
  deleteRequest = output<Todo>();

  protected handleToggle(): void {
    this.toggleComplete.emit(this.todo());
  }

  protected handleEdit(): void {
    this.editRequest.emit(this.todo());
  }

  protected handleDelete(): void {
    this.deleteRequest.emit(this.todo());
  }
}
