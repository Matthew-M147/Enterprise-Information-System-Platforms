import { Injectable } from '@angular/core';
import { Order, OrderInput } from '../models/order';
import { CrudService } from './crud.service';

@Injectable({ providedIn: 'root' })
export class OrdersService extends CrudService<Order, OrderInput> {
  protected readonly url = '/api/orders';
}
