import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Runs for every HttpClient request (like an axios interceptor):
 * 1. adds "Authorization: Bearer <token>" if the user is logged in;
 * 2. if the API answers 401 (token expired or invalid), logs the user out.
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const token = authService.token();

  // Requests are immutable, so we send a modified copy.
  const authorizedRequest = token
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(authorizedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      // Without a token a 401 just means "wrong password" on the login page, so we only react when logged in.
      if (error.status === 401 && token) {
        authService.logout();
      }
      return throwError(() => error);
    }),
  );
};
