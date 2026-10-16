import { Component, computed, input, output } from '@angular/core';

/**
 * Presentational component: gets data through inputs and reports user actions through outputs.
 * input() ≈ props in React, output() ≈ a callback prop like onPageChange.
 */
@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.html',
})
export class Pagination {
  readonly page = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly pageChange = output<number>();

  /** Up to 5 page numbers around the current page, e.g. [3, 4, 5, 6, 7]. */
  readonly pages = computed(() => {
    const total = this.totalPages();
    const end = Math.min(total, Math.max(this.page() + 2, 5));
    const start = Math.max(1, end - 4);
    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  });

  goTo(page: number): void {
    if (page >= 1 && page <= this.totalPages() && page !== this.page()) {
      this.pageChange.emit(page);
    }
  }
}
