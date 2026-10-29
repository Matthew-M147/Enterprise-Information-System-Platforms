export interface Client {
  id: number;
  lastName: string;
  firstName: string;
  middleName: string | null;
  phone: string;
  city: string;
  street: string;
  building: string;
  apartment: string | null;
  ordersCount: number;
}

export type ClientInput = Omit<Client, 'id' | 'ordersCount'>;

export function fullName(c: Client): string {
  return [c.lastName, c.firstName, c.middleName].filter(Boolean).join(' ');
}

export function address(c: Client): string {
  return `${c.city}, ${c.street}, ${c.building}` + (c.apartment ? `, кв. ${c.apartment}` : '');
}
