import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StockNotificationService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5243/api/StockNotifications';

  subscribeToStock(productId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${productId}/subscribe`, {});
  }

  checkSubscriptionStatus(productId: number): Observable<{ isSubscribed: boolean }> {
    return this.http.get<{ isSubscribed: boolean }>(`${this.apiUrl}/${productId}/status`);
  }
}
