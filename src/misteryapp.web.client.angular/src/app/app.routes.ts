import { Routes } from '@angular/router'
import { authGuard } from './core/guards/auth.guard'

export const routes: Routes = [
  { path: '', redirectTo: '/todos', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/auth/login-page/login-page.component').then(
        (m) => m.LoginPageComponent
      ),
  },
  {
    path: 'todos',
    loadComponent: () =>
      import('./pages/todos/todos-page/todos-page.component').then(
        (m) => m.TodosPageComponent
      ),
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: '/todos' },
]
