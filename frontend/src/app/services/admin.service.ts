import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product';

export interface DashboardStatsDto {
  dailyViews: number;
  dailyFavorites: number;
  dailyCartAdditions: number;
  dailyRevenue: number;
  dailyOrdersCount: number;
  weeklyRevenue: number;
  weeklyOrdersCount: number;
  monthlyRevenue: number;
  monthlyOrdersCount: number;
}

export interface AdminOrderDto {
  id: number;
  userId: string;
  userEmail: string;
  userFullName: string;
  status: string;
  totalAmount: number;
  createdAt: string;
  shippingAddress?: string;
  shippingCompany?: string;
  trackingNumber?: string;
  trackingUrl?: string;
  items: AdminOrderItemDto[];
}

export interface AdminOrderItemDto {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly adminApiUrl = 'http://localhost:5243/api/admin';
  private readonly productsApiUrl = 'http://localhost:5243/api/products';
  private readonly dashboardApiUrl = 'http://localhost:5243/api/dashboard';

  constructor(private http: HttpClient) {}

  // Dashboard Stats
  getDashboardStats(): Observable<DashboardStatsDto> {
    return this.http.get<DashboardStatsDto>(`${this.dashboardApiUrl}/stats`);
  }

  getLowStockProducts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.dashboardApiUrl}/low-stock`);
  }

  // Orders
  getOrders(): Observable<AdminOrderDto[]> {
    return this.http.get<AdminOrderDto[]>(`${this.adminApiUrl}/orders`);
  }

  updateOrderStatus(orderId: number, status: string): Observable<{ message: string; status: string }> {
    return this.http.put<{ message: string; status: string }>(`${this.adminApiUrl}/orders/${orderId}/status`, { status });
  }

  updateOrderShipping(orderId: number, shippingData: { shippingCompany: string, trackingNumber: string, trackingUrl?: string }): Observable<any> {
    return this.http.put(`${this.adminApiUrl}/orders/${orderId}/shipping`, shippingData);
  }

  // Products
  createProduct(product: Omit<Product, 'id'>): Observable<Product> {
    return this.http.post<Product>(this.productsApiUrl, product);
  }

  updateProduct(product: Product): Observable<void> {
    return this.http.put<void>(`${this.productsApiUrl}/${product.id}`, product);
  }

  deleteProduct(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.productsApiUrl}/${productId}`);
  }
}
