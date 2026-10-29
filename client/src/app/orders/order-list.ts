import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Order } from '../models/order';
import { OrdersService } from '../services/orders.service';

@Component({
  selector: 'app-order-list',
  imports: [RouterLink, DecimalPipe, DatePipe],
  templateUrl: './order-list.html'
})
export class OrderList {
  private readonly service = inject(OrdersService);

  protected readonly orders = signal<Order[] | null>(null);
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
      error: () => this.error.set('Не вдалося видалити замовлення')
    });
  }

  private load(): void {
    this.service.getAll().subscribe({
      next: orders => this.orders.set(orders),
      error: () => this.error.set('Не вдалося завантажити замовлення. Перевірте, що Web API запущено')
    });
  }
}
