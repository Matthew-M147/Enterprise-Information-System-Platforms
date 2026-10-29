import { Injectable } from '@angular/core';
import { Client, ClientInput } from '../models/client';
import { CrudService } from './crud.service';

@Injectable({ providedIn: 'root' })
export class ClientsService extends CrudService<Client, ClientInput> {
  protected readonly url = '/api/clients';
}
