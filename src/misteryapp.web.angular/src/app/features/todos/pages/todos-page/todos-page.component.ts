import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { TodoStateService } from '../../state/todo-state.service';
import { GlassButtonComponent } from '../../../../shared/components/glass-button/glass-button.component';
import { GlassCardComponent } from '../../../../shared/components/glass-card/glass-card.component';
import { TodoListComponent } from '../../components/todo-list/todo-list.component';
import { TodoFormComponent } from '../../components/todo-form/todo-form.component';
import { TodoFilterBarComponent } from '../../components/todo-filter-bar/todo-filter-bar.component';
import { CreateTodoRequest } from '../../../../domain/todo.model';

@Component({
  selector: 'app-todos-page',
  templateUrl: './todos-page.component.html',
  styleUrl: './todos-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    GlassButtonComponent,
    GlassCardComponent,
    TodoListComponent,
    TodoFormComponent,
    TodoFilterBarComponent,
  ],
})
export class TodosPageComponent {
  protected readonly state = inject(TodoStateService);
  protected readonly formError = signal<string | null>(null);
  protected readonly isSaving = signal(false);

  protected async handleFormSubmit(req: CreateTodoRequest): Promise<void> {
    this.isSaving.set(true);
    this.formError.set(null);

    const editingTodo = this.state.editingTodo();
    const error = editingTodo
      ? await this.state.updateTodo(editingTodo.id, req)
      : await this.state.createTodo(req);

    this.isSaving.set(false);
    if (error) this.formError.set(error);
  }
}
