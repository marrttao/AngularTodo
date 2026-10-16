import { DatePipe } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { Task } from '../../../core/models/task.models';

/** Today's date as "YYYY-MM-DD" in the user's local time zone. */
function todayIso(): string {
  const now = new Date();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  return `${now.getFullYear()}-${month}-${day}`;
}

/** One row of the task list. */
@Component({
  selector: 'app-task-item',
  imports: [DatePipe],
  templateUrl: './task-item.html',
})
export class TaskItem {
  readonly task = input.required<Task>();
  readonly selected = input(false);

  readonly toggle = output<Task>();
  readonly open = output<Task>();
  readonly remove = output<Task>();

  // "YYYY-MM-DD" strings can be compared alphabetically.
  readonly isOverdue = computed(() => {
    const task = this.task();
    return !task.isCompleted && task.dueDate !== null && task.dueDate < todayIso();
  });
}
