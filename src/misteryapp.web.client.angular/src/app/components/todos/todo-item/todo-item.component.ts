import { ChangeDetectionStrategy, Component, input, output } from '@angular/core'
import { TodoItem } from '../../../domain/models/todo-item'

@Component({
  selector: 'app-todo-item',
  templateUrl: './todo-item.component.html',
  styleUrl: './todo-item.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoItemComponent {
  todo = input.required<TodoItem>()
  toggle = output<number>()
  delete = output<number>()
}
