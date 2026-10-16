import { Component, ElementRef, effect, input, output, signal, viewChild } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Category } from '../../../core/models/category.models';

/**
 * Left sidebar with "All tasks" and the user's categories.
 * It doesn't call the API itself: it emits events and the parent page does the work.
 */
@Component({
  selector: 'app-category-sidebar',
  imports: [ReactiveFormsModule],
  templateUrl: './category-sidebar.html',
})
export class CategorySidebar {
  readonly categories = input.required<Category[]>();
  readonly selectedId = input<number | null>(null);

  readonly select = output<number | null>();
  readonly create = output<string>();
  readonly rename = output<{ id: number; name: string }>();
  readonly remove = output<Category>();

  readonly newCategory = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(50)],
  });

  /** Id of the category whose name is being edited right now. */
  readonly editingId = signal<number | null>(null);
  readonly editName = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(50)],
  });

  // Reference to the rename <input #editInput> (like useRef in React); undefined while it's not rendered.
  private readonly editInput = viewChild<ElementRef<HTMLInputElement>>('editInput');

  constructor() {
    // Focus the rename input as soon as it appears on the screen.
    effect(() => this.editInput()?.nativeElement.focus());
  }

  addCategory(): void {
    const name = this.newCategory.value.trim();
    if (!name || this.newCategory.invalid) {
      return;
    }
    this.create.emit(name);
    this.newCategory.reset();
  }

  startEdit(category: Category): void {
    this.editingId.set(category.id);
    this.editName.setValue(category.name);
  }

  saveEdit(category: Category): void {
    // Enter and blur can both fire for the same edit; handle it only once.
    if (this.editingId() !== category.id) {
      return;
    }

    const name = this.editName.value.trim();
    if (name && this.editName.valid && name !== category.name) {
      this.rename.emit({ id: category.id, name });
    }
    this.editingId.set(null);
  }

  cancelEdit(): void {
    this.editingId.set(null);
  }
}
