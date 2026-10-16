import { HttpErrorResponse } from '@angular/common/http';

/**
 * Turns an API error (ProblemDetails / ValidationProblemDetails) into a readable message.
 */
export function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'Cannot reach the server. Is the API running?';
    }

    const body = error.error;

    // 400 from [ApiController] validation: { errors: { Title: ["..."] } }
    if (body?.errors) {
      return Object.values(body.errors as Record<string, string[]>).flat().join(' ');
    }
    // Errors from ExceptionHandlingMiddleware: { title, status, detail }
    if (body?.detail) {
      return body.detail;
    }
  }
  return 'Something went wrong. Please try again.';
}
