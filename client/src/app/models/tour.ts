export interface Tour {
  id: number;
  name: string;
  description: string;
  price: number;
  durationDays: number;
  ordersCount: number;
}

export type TourInput = Omit<Tour, 'id' | 'ordersCount'>;
