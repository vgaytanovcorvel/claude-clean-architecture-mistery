import { ChangeDetectionStrategy, Component, input, output } from '@angular/core'
import { TodoItem } from '../../../domain/models/todo-item'
import { TodoItemComponent } from '../todo-item/todo-item.component'

@Component({
  selector: 'app-todo-list-view',
  templateUrl: './todo-list-view.component.html',
  styleUrl: './todo-list-view.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TodoItemComponent],
})
export class TodoListViewComponent {
  todos = input.required<readonly TodoItem[]>()
  isLoading = input(false)
  toggle = output<number>()
  delete = output<number>()
}
