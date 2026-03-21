import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Todo } from '../../../../domain/todo.model';
import { TodoCardComponent } from '../todo-card/todo-card.component';

@Component({
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrl: './todo-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TodoCardComponent],
})
export class TodoListComponent {
  todos = input.required<readonly Todo[]>();
  isLoading = input<boolean>(false);

  toggleComplete = output<Todo>();
  editRequest = output<Todo>();
  deleteRequest = output<Todo>();
}
