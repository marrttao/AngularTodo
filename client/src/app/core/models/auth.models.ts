// TypeScript interfaces that mirror the API DTOs (TodoApp.Interfaces/DTOs/Auth).

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  email: string;
}
