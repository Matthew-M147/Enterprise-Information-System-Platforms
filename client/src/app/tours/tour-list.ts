import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Tour } from '../models/tour';
import { ToursService } from '../services/tours.service';

@Component({
  selector: 'app-tour-list',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './tour-list.html'
})
export class TourList {
  private readonly service = inject(ToursService);

  protected readonly tours = signal<Tour[] | null>(null);
  protected readonly deletingId = signal<number | null>(null);
  protected readonly error = signal<string | null>(null);

  constructor() {
    this.load();
  }

  protected remove(id: number): void {
    this.service.delete(id).subscribe({
      next: () => {
        this.deletingId.set(null);
        this.load();
      },
      error: () => this.error.set('Не вдалося видалити тур')
    });
  }

  private load(): void {
    this.service.getAll().subscribe({
      next: tours => this.tours.set(tours),
      error: () => this.error.set('Не вдалося завантажити тури. Перевірте, що Web API запущено')
    });
  }
}
