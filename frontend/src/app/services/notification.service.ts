import { Injectable, inject, signal, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Notification } from '../models/notification';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class NotificationService implements OnDestroy {
  private readonly apiUrl = 'http://localhost:5243/api/notifications';
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  notifications = signal<Notification[]>([]);
  unreadCount = signal<number>(0);

  private pollingInterval: any = null;

  startPolling(): void {
    this.stopPolling();
    if (!this.authService.isLoggedIn()) return;

    this.fetchUnreadCount();
    this.pollingInterval = setInterval(() => {
      if (this.authService.isLoggedIn()) {
        this.fetchUnreadCount();
      } else {
        this.stopPolling();
      }
    }, 30000); // 30 saniye
  }

  stopPolling(): void {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
      this.pollingInterval = null;
    }
  }

  fetchNotifications(): void {
    this.http.get<Notification[]>(this.apiUrl).subscribe({
      next: (data) => this.notifications.set(data),
      error: () => {}
    });
  }

  fetchUnreadCount(): void {
    this.http.get<{ count: number }>(`${this.apiUrl}/unread-count`).subscribe({
      next: (data) => this.unreadCount.set(data.count),
      error: () => {}
    });
  }

  markAsRead(id: number): void {
    this.http.put(`${this.apiUrl}/${id}/read`, {}).subscribe({
      next: () => {
        this.notifications.update(list =>
          list.map(n => n.id === id ? { ...n, isRead: true } : n)
        );
        this.unreadCount.update(c => Math.max(0, c - 1));
      }
    });
  }

  markAllAsRead(): void {
    this.http.put(`${this.apiUrl}/read-all`, {}).subscribe({
      next: () => {
        this.notifications.update(list =>
          list.map(n => ({ ...n, isRead: true }))
        );
        this.unreadCount.set(0);
      }
    });
  }

  ngOnDestroy(): void {
    this.stopPolling();
  }
}
