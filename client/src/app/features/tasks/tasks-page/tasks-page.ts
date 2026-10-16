import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { EMPTY, Subject, catchError, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { Category } from '../../../core/models/category.models';
import { PagedResult, Task, TaskQuery, UpdateTaskRequest } from '../../../core/models/task.models';
import { AuthService } from '../../../core/services/auth.service';
import { CategoryService } from '../../../core/services/category.service';
import { TaskService } from '../../../core/services/task.service';
import { getErrorMessage } from '../../../core/utils/error-message';
import { Pagination } from '../../../shared/components/pagination/pagination';
import { CategorySidebar } from '../../categories/category-sidebar/category-sidebar';
import { TaskDetails } from '../task-details/task-details';
import { TaskItem } from '../task-item/task-item';

type StatusFilter = 'all' | 'active' | 'completed';

const PAGE_SIZE = 10;

/**
 * Main page ("container" component): owns the state, talks to the services
 * and passes data down to the presentational child components.
 */
@Component({
  selector: 'app-tasks-page',
  imports: [ReactiveFormsModule, CategorySidebar, TaskItem, TaskDetails, Pagination],
  templateUrl: './tasks-page.html',
})
export class TasksPage {
  private readonly taskService = inject(TaskService);
  private readonly categoryService = inject(CategoryService);
  readonly authService = inject(AuthService);

  // ---- State (signals ≈ useState) ----
  readonly categories = signal<Category[]>([]);
  readonly selectedCategoryId = signal<number | null>(null);
  readonly status = signal<StatusFilter>('all');
  readonly page = signal(1);
  readonly result = signal<PagedResult<Task> | null>(null);
  readonly selectedTask = signal<Task | null>(null);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly newTaskControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(200)],
  });

  /** Emits every time the task list has to be reloaded. */
  private readonly reload$ = new Subject<void>();

  readonly statusFilters: { value: StatusFilter; label: string }[] = [
    { value: 'all', label: 'All' },
    { value: 'active', label: 'Active' },
    { value: 'completed', label: 'Completed' },
  ];

  // ---- Derived state (computed ≈ useMemo) ----
  readonly title = computed(() => {
    const id = this.selectedCategoryId();
    if (id === null) {
      return 'All tasks';
    }
    return this.categories().find((c) => c.id === id)?.name ?? 'Tasks';
  });

  constructor() {
    // switchMap: when a new reload starts, the previous request is cancelled,
    // so a slow old response can never overwrite newer results (e.g. after quick filter clicks).
    this.reload$
      .pipe(
        switchMap(() =>
          this.taskService.getTasks(this.buildQuery()).pipe(
            // Handle the error here, so the outer stream stays alive for the next reloads.
            catchError((err) => {
              this.showError(err);
              return EMPTY;
            }),
          ),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((result) => this.onTasksLoaded(result));

    this.loadCategories();
    this.loadTasks();

    // Search: wait until the user stops typing for 300 ms, then reload.
    // takeUntilDestroyed() unsubscribes automatically when the component is destroyed.
    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.loadTasks();
      });
  }

  // ---- Loading data ----

  loadTasks(): void {
    this.loading.set(true);
    this.reload$.next();
  }

  /** Builds the API query from the current filters. */
  private buildQuery(): TaskQuery {
    const status = this.status();
    return {
      page: this.page(),
      pageSize: PAGE_SIZE,
      search: this.searchControl.value.trim(),
      categoryId: this.selectedCategoryId(),
      isCompleted: status === 'all' ? null : status === 'completed',
    };
  }

  private onTasksLoaded(result: PagedResult<Task>): void {
    // The current page can become empty, e.g. after deleting its last task: go to the last existing page.
    if (result.items.length === 0 && result.totalPages > 0 && this.page() > result.totalPages) {
      this.page.set(result.totalPages);
      this.loadTasks();
      return;
    }
    this.result.set(result);
    this.loading.set(false);
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (categories) => this.categories.set(categories),
      error: (err) => this.showError(err),
    });
  }

  // ---- Filters and pagination ----

  selectCategory(id: number | null): void {
    this.selectedCategoryId.set(id);
    this.selectedTask.set(null);
    this.page.set(1);
    this.loadTasks();
  }

  setStatus(status: StatusFilter): void {
    this.status.set(status);
    this.page.set(1);
    this.loadTasks();
  }

  changePage(page: number): void {
    this.page.set(page);
    this.loadTasks();
  }

  // ---- Tasks ----

  addTask(): void {
    const title = this.newTaskControl.value.trim();
    if (!title || this.newTaskControl.invalid) {
      return;
    }

    // Clear the input right away, so the user can type the next task while this one is being saved.
    this.newTaskControl.reset();

    // A task created inside a category belongs to that category.
    this.taskService.create({ title, categoryId: this.selectedCategoryId() }).subscribe({
      next: () => {
        this.page.set(1); // new tasks are shown first
        this.loadTasks();
      },
      error: (err) => {
        this.newTaskControl.setValue(title); // give the text back so it isn't lost
        this.showError(err);
      },
    });
  }

  toggleTask(task: Task): void {
    const request: UpdateTaskRequest = {
      title: task.title,
      description: task.description,
      isCompleted: !task.isCompleted,
      dueDate: task.dueDate,
      categoryId: task.categoryId,
    };

    this.taskService.update(task.id, request).subscribe({
      next: (updated) => {
        if (this.selectedTask()?.id === updated.id) {
          this.selectedTask.set(updated);
        }
        this.loadTasks();
      },
      error: (err) => this.showError(err),
    });
  }

  openTask(task: Task): void {
    this.selectedTask.set(task);
  }

  saveTask(request: UpdateTaskRequest): void {
    const task = this.selectedTask();
    if (!task) {
      return;
    }

    this.saving.set(true);
    this.taskService.update(task.id, request).subscribe({
      next: (updated) => {
        this.selectedTask.set(updated);
        this.saving.set(false);
        this.loadTasks();
      },
      error: (err) => this.showError(err),
    });
  }

  deleteTask(task: Task): void {
    if (!confirm(`Delete "${task.title}"?`)) {
      return;
    }

    this.taskService.delete(task.id).subscribe({
      next: () => {
        if (this.selectedTask()?.id === task.id) {
          this.selectedTask.set(null);
        }
        this.loadTasks();
      },
      error: (err) => this.showError(err),
    });
  }

  // ---- Categories ----

  createCategory(name: string): void {
    this.categoryService.create(name).subscribe({
      next: (category) => {
        this.loadCategories();
        this.selectCategory(category.id);
      },
      error: (err) => this.showError(err),
    });
  }

  renameCategory(change: { id: number; name: string }): void {
    this.categoryService.update(change.id, change.name).subscribe({
      next: () => {
        this.loadCategories();
        this.loadTasks(); // tasks show the category name
      },
      error: (err) => this.showError(err),
    });
  }

  deleteCategory(category: Category): void {
    if (!confirm(`Delete list "${category.name}"? Its tasks will be kept without a category.`)) {
      return;
    }

    this.categoryService.delete(category.id).subscribe({
      next: () => {
        if (this.selectedCategoryId() === category.id) {
          this.selectedCategoryId.set(null);
          this.page.set(1);
        }
        this.selectedTask.set(null);
        this.loadCategories();
        this.loadTasks();
      },
      error: (err) => this.showError(err),
    });
  }

  // ---- Helpers ----

  private showError(err: unknown): void {
    this.error.set(getErrorMessage(err));
    this.loading.set(false);
    this.saving.set(false);
  }
}
