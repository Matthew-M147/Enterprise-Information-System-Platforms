export interface Order {
  id: number;
  clientId: number;
  clientName: string;
  tourId: number;
  tourName: string;
  orderDate: string;
  tripDate: string;
  durationDays: number;
  quantity: number;
  price: number;
  discountPercent: number;
  total: number;
}

export interface OrderInput {
  clientId: number;
  tourId: number;
  orderDate: string;
  tripDate: string;
  quantity: number;
  discountPercent: number;
}
