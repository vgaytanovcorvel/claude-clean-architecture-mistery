import { ChangeDetectionStrategy, Component, inject } from '@angular/core'
import { TodoStateService } from '../../../state/todos/todo-state.service'
import { AuthStateService } from '../../../state/auth/auth-state.service'
import { TodoInputComponent } from '../../../components/todos/todo-input/todo-input.component'
import { TodoListViewComponent } from '../../../components/todos/todo-list-view/todo-list-view.component'
import { TodoFilterComponent } from '../../../components/todos/todo-filter/todo-filter.component'
import { CreateTodoRequest } from '../../../domain/models/requests'

@Component({
  selector: 'app-todos-page',
  templateUrl: './todos-page.component.html',
  styleUrl: './todos-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TodoInputComponent, TodoListViewComponent, TodoFilterComponent],
})
export class TodosPageComponent {
  protected readonly todoState = inject(TodoStateService)
  protected readonly authState = inject(AuthStateService)

  protected async onCreateTodo(request: CreateTodoRequest) {
    await this.todoState.createTodo(request)
  }

  protected async onToggle(id: number) {
    await this.todoState.toggleTodo(id)
  }

  protected async onDelete(id: number) {
    await this.todoState.deleteTodo(id)
  }
}
