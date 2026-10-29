import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { address, Client, fullName } from '../models/client';
import { ClientsService } from '../services/clients.service';

@Component({
  selector: 'app-client-list',
  imports: [RouterLink],
  templateUrl: './client-list.html'
})
export class ClientList {
  private readonly service = inject(ClientsService);

  protected readonly clients = signal<Client[] | null>(null);
  protected readonly deletingId = signal<number | null>(null);
  protected readonly error = signal<string | null>(null);
  protected readonly fullName = fullName;
  protected readonly address = address;

  constructor() {
    this.load();
  }

  protected remove(id: number): void {
    this.service.delete(id).subscribe({
      next: () => {
        this.deletingId.set(null);
        this.load();
      },
      error: () => this.error.set('Не вдалося видалити клієнта')
    });
  }

  private load(): void {
    this.service.getAll().subscribe({
      next: clients => this.clients.set(clients),
      error: () => this.error.set('Не вдалося завантажити клієнтів. Перевірте, що Web API запущено')
    });
  }
}
