import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface OrderItemResponse {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface OrderResponse {
  id: number;
  createdAt: string;
  status: string;
  totalAmount: number;
  shippingAddress?: string;
  shippingCompany?: string;
  trackingNumber?: string;
  trackingUrl?: string;
  items: OrderItemResponse[];
}

export interface CreateOrderItem {
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface CreateOrderDto {
  shippingAddress?: string;
  items: CreateOrderItem[];
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly apiUrl = 'http://localhost:5243/api/orders';

  constructor(private http: HttpClient) {}

  getOrders(): Observable<OrderResponse[]> {
    return this.http.get<OrderResponse[]>(this.apiUrl);
  }

  getOrder(id: number): Observable<OrderResponse> {
    return this.http.get<OrderResponse>(`${this.apiUrl}/${id}`);
  }

  createOrder(dto: CreateOrderDto): Observable<{ id: number; message: string }> {
    return this.http.post<{ id: number; message: string }>(this.apiUrl, dto);
  }
}
