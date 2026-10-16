import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Route guards run before navigation. Returning a UrlTree means "redirect there instead".
 * In React this is usually a <ProtectedRoute> wrapper component.
 */

/** Allows the route only for logged-in users. */
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isLoggedIn() ? true : router.createUrlTree(['/login']);
};

/** Allows the route only for guests (login/register pages). */
export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isLoggedIn() ? router.createUrlTree(['/tasks']) : true;
};
