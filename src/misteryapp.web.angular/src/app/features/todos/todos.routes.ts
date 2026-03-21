import { Routes } from '@angular/router';

export const TODOS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/todos-page/todos-page.component').then(
        (m) => m.TodosPageComponent
      ),
  },
];
