import { ChangeDetectionStrategy, Component, input, output, inject, OnChanges } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Todo, CreateTodoRequest, UpdateTodoRequest } from '../../../../domain/todo.model';
import { Priority, PRIORITIES, PRIORITY_LABELS } from '../../../../domain/priority.model';
import { GlassInputComponent } from '../../../../shared/components/glass-input/glass-input.component';
import { GlassButtonComponent } from '../../../../shared/components/glass-button/glass-button.component';

@Component({
  selector: 'app-todo-form',
  templateUrl: './todo-form.component.html',
  styleUrl: './todo-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, GlassInputComponent, GlassButtonComponent],
})
export class TodoFormComponent implements OnChanges {
  todo = input<Todo | null>(null);
  isLoading = input<boolean>(false);
  serverError = input<string | null>(null);

  submitted = output<CreateTodoRequest>();
  cancelled = output<void>();

  protected readonly priorities = PRIORITIES;
  protected readonly priorityLabels = PRIORITY_LABELS;

  private readonly fb = inject(FormBuilder);

  readonly form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    priority: ['medium' as Priority, Validators.required],
  });

  ngOnChanges(): void {
    const todo = this.todo();
    if (todo) {
      this.form.patchValue({
        title: todo.title,
        description: todo.description,
        priority: todo.priority,
      });
    } else {
      this.form.reset({ title: '', description: '', priority: 'medium' });
    }
  }

  protected get isEditing(): boolean {
    return this.todo() !== null;
  }

  protected getError(field: string): string | null {
    const control = this.form.get(field);
    if (!control?.invalid || !control.touched) return null;
    if (control.hasError('required')) return 'This field is required.';
    if (control.hasError('maxlength')) return 'Title is too long (max 200 characters).';
    return null;
  }

  protected handleSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    const { title, description, priority } = this.form.value;
    this.submitted.emit({
      title: title!,
      description: description ?? '',
      priority: priority as Priority,
    });
  }

  protected handleCancel(): void {
    this.cancelled.emit();
  }
}
