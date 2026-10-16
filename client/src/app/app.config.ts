import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

/**
 * Application-wide providers (the root of Angular's dependency injection).
 * Similar to wrapping a React app in <BrowserRouter> and context providers.
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    // HttpClient + our interceptor, which adds the JWT to every request.
    provideHttpClient(withInterceptors([authInterceptor])),
  ],
};
