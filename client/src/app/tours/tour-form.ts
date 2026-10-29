import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ApiErrors, GENERAL, parseApiErrors } from '../services/api-errors';
import { ToursService } from '../services/tours.service';

@Component({
  selector: 'app-tour-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './tour-form.html'
})
export class TourForm {
  private readonly service = inject(ToursService);
  private readonly router = inject(Router);
  private readonly id: number | null;

  protected readonly errors = signal<ApiErrors>({});
  protected readonly general = GENERAL;
  protected readonly title: string;
  protected readonly form = inject(NonNullableFormBuilder).group({
    name: '',
    description: '',
    price: 0,
    durationDays: 1
  });

  constructor() {
    const idParam = inject(ActivatedRoute).snapshot.paramMap.get('id');
    this.id = idParam ? Number(idParam) : null;
    this.title = this.id === null ? 'Новий тур' : 'Редагування туру';

    if (this.id !== null) {
      this.service.get(this.id).subscribe({
        next: t => this.form.patchValue(t),
        error: e => this.errors.set(parseApiErrors(e))
      });
    }
  }

  protected save(): void {
    const value = this.form.getRawValue();
    const request: Observable<unknown> = this.id === null ? this.service.create(value) : this.service.update(this.id, value);
    request.subscribe({
      next: () => this.router.navigate(['/tours']),
      error: e => this.errors.set(parseApiErrors(e))
    });
  }
}
