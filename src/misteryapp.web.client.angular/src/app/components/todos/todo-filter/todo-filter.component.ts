import { ChangeDetectionStrategy, Component, input, output } from '@angular/core'

@Component({
  selector: 'app-todo-filter',
  templateUrl: './todo-filter.component.html',
  styleUrl: './todo-filter.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoFilterComponent {
  filter = input.required<'all' | 'active' | 'completed'>()
  filterChange = output<'all' | 'active' | 'completed'>()
}
