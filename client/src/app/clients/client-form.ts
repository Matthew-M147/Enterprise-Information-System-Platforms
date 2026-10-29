import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { ApiErrors, GENERAL, parseApiErrors } from '../services/api-errors';
import { ClientsService } from '../services/clients.service';

@Component({
  selector: 'app-client-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './client-form.html'
})
export class ClientForm {
  private readonly service = inject(ClientsService);
  private readonly router = inject(Router);
  private readonly id: number | null;

  protected readonly errors = signal<ApiErrors>({});
  protected readonly general = GENERAL;
  protected readonly title: string;
  protected readonly form = inject(NonNullableFormBuilder).group({
    lastName: '',
    firstName: '',
    middleName: '',
    phone: '',
    city: '',
    street: '',
    building: '',
    apartment: ''
  });

  constructor() {
    const idParam = inject(ActivatedRoute).snapshot.paramMap.get('id');
    this.id = idParam ? Number(idParam) : null;
    this.title = this.id === null ? 'Новий клієнт' : 'Редагування клієнта';

    if (this.id !== null) {
      this.service.get(this.id).subscribe({
        next: c => this.form.patchValue({ ...c, middleName: c.middleName ?? '', apartment: c.apartment ?? '' }),
        error: e => this.errors.set(parseApiErrors(e))
      });
    }
  }

  protected save(): void {
    const value = this.form.getRawValue();
    const request: Observable<unknown> = this.id === null ? this.service.create(value) : this.service.update(this.id, value);
    request.subscribe({
      next: () => this.router.navigate(['/clients']),
      error: e => this.errors.set(parseApiErrors(e))
    });
  }
}
