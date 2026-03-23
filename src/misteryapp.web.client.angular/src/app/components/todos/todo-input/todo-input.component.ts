import { ChangeDetectionStrategy, Component, output, signal } from '@angular/core'
import { FormsModule } from '@angular/forms'
import { CreateTodoRequest } from '../../../domain/models/requests'

@Component({
  selector: 'app-todo-input',
  templateUrl: './todo-input.component.html',
  styleUrl: './todo-input.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule],
})
export class TodoInputComponent {
  create = output<CreateTodoRequest>()

  protected title = signal('')

  protected onSubmit() {
    const t = this.title().trim()
    if (!t) return

    this.create.emit({ title: t, description: null })
    this.title.set('')
  }
}
