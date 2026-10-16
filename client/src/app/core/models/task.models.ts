export interface Task {
  id: number;
  title: string;
  description: string | null;
  isCompleted: boolean;
  /** Calendar date in "YYYY-MM-DD" format (DateOnly on the server). */
  dueDate: string | null;
  createdAt: string;
  categoryId: number | null;
  categoryName: string | null;
}

export interface CreateTaskRequest {
  title: string;
  description?: string | null;
  dueDate?: string | null;
  categoryId?: number | null;
}

export interface UpdateTaskRequest {
  title: string;
  description: string | null;
  isCompleted: boolean;
  dueDate: string | null;
  categoryId: number | null;
}

/** Query parameters of GET /api/tasks. */
export interface TaskQuery {
  page: number;
  pageSize: number;
  search?: string;
  categoryId?: number | null;
  isCompleted?: boolean | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
