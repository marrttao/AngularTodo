import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import {
  CreateTaskRequest,
  PagedResult,
  Task,
  TaskQuery,
  UpdateTaskRequest,
} from '../models/task.models';

/** Thin wrapper around the /api/tasks endpoints. */
@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/tasks`;

  /** GET /api/tasks?page=1&pageSize=10&search=...&categoryId=...&isCompleted=... */
  getTasks(query: TaskQuery): Observable<PagedResult<Task>> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);

    // Only send the filters that are actually set.
    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.categoryId != null) {
      params = params.set('categoryId', query.categoryId);
    }
    if (query.isCompleted != null) {
      params = params.set('isCompleted', query.isCompleted);
    }

    return this.http.get<PagedResult<Task>>(this.url, { params });
  }

  create(request: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(this.url, request);
  }

  update(id: number, request: UpdateTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.url}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
