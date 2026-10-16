import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { API_URL } from '../api.config';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.models';

const TOKEN_KEY = 'todo.token';
const EXPIRES_AT_KEY = 'todo.expiresAt';
const EMAIL_KEY = 'todo.email';

/**
 * Keeps the login state of the app.
 * providedIn: 'root' = a single shared instance for the whole app (like a React context).
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  // Signals hold state; anything that reads them (templates, computed) updates automatically.
  private readonly tokenSignal = signal<string | null>(this.loadValidToken());
  private readonly emailSignal = signal<string | null>(localStorage.getItem(EMAIL_KEY));

  readonly token = this.tokenSignal.asReadonly();
  readonly email = this.emailSignal.asReadonly();
  readonly isLoggedIn = computed(() => this.tokenSignal() !== null);

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_URL}/auth/login`, request)
      .pipe(tap((response) => this.saveSession(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_URL}/auth/register`, request)
      .pipe(tap((response) => this.saveSession(response)));
  }

  /**
   * JWT is stateless: the server doesn't store sessions, so there is nothing to call on logout.
   * We simply forget the token on the client.
   */
  logout(): void {
    this.clearStorage();
    this.tokenSignal.set(null);
    this.emailSignal.set(null);
    this.router.navigate(['/login']);
  }

  private saveSession(response: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(EXPIRES_AT_KEY, response.expiresAt);
    localStorage.setItem(EMAIL_KEY, response.email);
    this.tokenSignal.set(response.token);
    this.emailSignal.set(response.email);
  }

  /** Returns the saved token, or null if there is none or it has already expired. */
  private loadValidToken(): string | null {
    const token = localStorage.getItem(TOKEN_KEY);
    const expiresAt = localStorage.getItem(EXPIRES_AT_KEY);

    if (!token || !expiresAt || new Date(expiresAt) <= new Date()) {
      this.clearStorage();
      return null;
    }
    return token;
  }

  private clearStorage(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRES_AT_KEY);
    localStorage.removeItem(EMAIL_KEY);
  }
}
