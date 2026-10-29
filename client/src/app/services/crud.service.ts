import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';

export abstract class CrudService<T, TInput> {
  protected readonly http = inject(HttpClient);
  protected abstract readonly url: string;

  getAll(): Observable<T[]> {
    return this.http.get<T[]>(this.url);
  }

  get(id: number): Observable<T> {
    return this.http.get<T>(`${this.url}/${id}`);
  }

  create(item: TInput): Observable<T> {
    return this.http.post<T>(this.url, item);
  }

  update(id: number, item: TInput): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, item);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
