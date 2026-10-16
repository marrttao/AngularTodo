import { DatePipe } from '@angular/common';
import { Component, effect, inject, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Category } from '../../../core/models/category.models';
import { Task, UpdateTaskRequest } from '../../../core/models/task.models';

/** Right-side panel for editing a task (like the details pane in Microsoft To-Do). */
@Component({
  selector: 'app-task-details',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './task-details.html',
})
export class TaskDetails {
  private readonly fb = inject(FormBuilder);

  readonly task = input.required<Task>();
  readonly categories = input.required<Category[]>();
  readonly saving = input(false);

  readonly save = output<UpdateTaskRequest>();
  readonly remove = output<Task>();
  readonly close = output<void>();

  readonly form = this.fb.group({
    title: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(200)]),
    description: this.fb.control<string | null>(null, [Validators.maxLength(2000)]),
    isCompleted: this.fb.nonNullable.control(false),
    dueDate: this.fb.control<string | null>(null),
    categoryId: this.fb.control<number | null>(null),
  });

  constructor() {
    // effect() re-runs whenever a signal it reads changes (similar to useEffect with [task]).
    // When another task is selected, fill the form with its values.
    effect(() => {
      const task = this.task();
      this.form.reset({
        title: task.title,
        description: task.description,
        isCompleted: task.isCompleted,
        dueDate: task.dueDate,
        categoryId: task.categoryId,
      });
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.save.emit({
      title: value.title,
      description: value.description || null,
      isCompleted: value.isCompleted,
      dueDate: value.dueDate || null, // an empty date input gives "", the API expects null
      categoryId: value.categoryId,
    });
  }
}
