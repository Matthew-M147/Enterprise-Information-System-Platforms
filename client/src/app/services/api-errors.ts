import { HttpErrorResponse } from '@angular/common/http';

export type ApiErrors = Record<string, string[]>;

export const GENERAL = '_';

export function parseApiErrors(error: HttpErrorResponse): ApiErrors {
  if (error.status === 400 && error.error?.errors) {
    const result: ApiErrors = {};
    for (const [key, messages] of Object.entries<string[]>(error.error.errors)) {
      const field = /^[A-Z][A-Za-z]*$/.test(key) ? key[0].toLowerCase() + key.slice(1) : GENERAL;
      result[field] = [...(result[field] ?? []), ...messages];
    }
    return result;
  }
  if (error.status === 404) {
    return { [GENERAL]: ['Запис не знайдено'] };
  }
  if (error.status === 0) {
    return { [GENERAL]: ['Сервер недоступний. Перевірте, що Web API запущено'] };
  }
  return { [GENERAL]: [`Помилка сервера (${error.status})`] };
}
