import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { Client, fullName } from '../models/client';
import { Tour } from '../models/tour';
import { ApiErrors, GENERAL, parseApiErrors } from '../services/api-errors';
import { ClientsService } from '../services/clients.service';
import { OrdersService } from '../services/orders.service';
import { ToursService } from '../services/tours.service';

@Component({
  selector: 'app-order-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './order-form.html'
})
export class OrderForm {
  private readonly service = inject(OrdersService);
  private readonly router = inject(Router);
  private readonly id: number | null;

  protected readonly clients = signal<Client[]>([]);
  protected readonly tours = signal<Tour[]>([]);
  protected readonly errors = signal<ApiErrors>({});
  protected readonly general = GENERAL;
  protected readonly fullName = fullName;
  protected readonly title: string;
  protected readonly form = inject(NonNullableFormBuilder).group({
    clientId: 0,
    tourId: 0,
    orderDate: today(),
    tripDate: today(),
    quantity: 1,
    discountPercent: 0
  });

  constructor() {
    const idParam = inject(ActivatedRoute).snapshot.paramMap.get('id');
    this.id = idParam ? Number(idParam) : null;
    this.title = this.id === null ? 'Нове замовлення' : `Редагування замовлення №${this.id}`;

    inject(ClientsService).getAll().subscribe(clients => this.clients.set(clients));
    inject(ToursService).getAll().subscribe(tours => this.tours.set(tours));

    if (this.id !== null) {
      this.service.get(this.id).subscribe({
        next: o => this.form.patchValue(o),
        error: e => this.errors.set(parseApiErrors(e))
      });
    }
  }

  protected save(): void {
    const value = this.form.getRawValue();
    const request: Observable<unknown> = this.id === null ? this.service.create(value) : this.service.update(this.id, value);
    request.subscribe({
      next: () => this.router.navigate(['/orders']),
      error: e => this.errors.set(parseApiErrors(e))
    });
  }
}

function today(): string {
  const now = new Date();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  return `${now.getFullYear()}-${month}-${day}`;
}
