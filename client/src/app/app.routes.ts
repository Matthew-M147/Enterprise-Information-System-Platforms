import { Routes } from '@angular/router';
import { ClientForm } from './clients/client-form';
import { ClientList } from './clients/client-list';
import { OrderForm } from './orders/order-form';
import { OrderList } from './orders/order-list';
import { TourForm } from './tours/tour-form';
import { TourList } from './tours/tour-list';

export const routes: Routes = [
  { path: '', redirectTo: 'clients', pathMatch: 'full' },
  { path: 'clients', component: ClientList, title: 'Клієнти' },
  { path: 'clients/new', component: ClientForm, title: 'Новий клієнт' },
  { path: 'clients/:id/edit', component: ClientForm, title: 'Редагування клієнта' },
  { path: 'tours', component: TourList, title: 'Тури' },
  { path: 'tours/new', component: TourForm, title: 'Новий тур' },
  { path: 'tours/:id/edit', component: TourForm, title: 'Редагування туру' },
  { path: 'orders', component: OrderList, title: 'Замовлення' },
  { path: 'orders/new', component: OrderForm, title: 'Нове замовлення' },
  { path: 'orders/:id/edit', component: OrderForm, title: 'Редагування замовлення' },
  { path: '**', redirectTo: 'clients' }
];
