import { Injectable } from '@angular/core';
import { Tour, TourInput } from '../models/tour';
import { CrudService } from './crud.service';

@Injectable({ providedIn: 'root' })
export class ToursService extends CrudService<Tour, TourInput> {
  protected readonly url = '/api/tours';
}
