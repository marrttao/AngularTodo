import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';
import { LoginPage } from './features/auth/login-page/login-page';
import { RegisterPage } from './features/auth/register-page/register-page';
import { TasksPage } from './features/tasks/tasks-page/tasks-page';

export const routes: Routes = [
  // Login and registration are only for guests; a logged-in user is sent to /tasks.
  { path: 'login', component: LoginPage, canActivate: [guestGuard], title: 'Log in · To Do' },
  { path: 'register', component: RegisterPage, canActivate: [guestGuard], title: 'Sign up · To Do' },

  // Protected page: without a token the guard redirects to /login.
  { path: 'tasks', component: TasksPage, canActivate: [authGuard], title: 'To Do' },

  { path: '', redirectTo: 'tasks', pathMatch: 'full' },
  { path: '**', redirectTo: 'tasks' },
];
