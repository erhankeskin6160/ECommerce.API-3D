import { Component, inject, OnInit, OnDestroy, signal, HostListener } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { LanguageService, SupportedLang } from '../../services/language.service';
import { NotificationService } from '../../services/notification.service';
import { WishlistService } from '../../services/wishlist.service';
import { Notification } from '../../models/notification';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, TranslateModule],
  templateUrl: './nav-bar.html',
  styleUrl: './nav-bar.scss'
})
export class NavBar implements OnInit, OnDestroy {
  cartService = inject(CartService);
  authService = inject(AuthService);
  langService = inject(LanguageService);
  notificationService = inject(NotificationService);
  wishlistService = inject(WishlistService);

  showNotifications = signal(false);

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.notificationService.startPolling();
      this.wishlistService.fetchCount();
    }
  }

  ngOnDestroy(): void {
    this.notificationService.stopPolling();
  }

  setLang(lang: SupportedLang): void {
    this.langService.setLanguage(lang);
  }

  toggleNotifications(): void {
    const isOpen = !this.showNotifications();
    this.showNotifications.set(isOpen);
    if (isOpen) {
      this.notificationService.fetchNotifications();
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.notification-wrapper')) {
      this.showNotifications.set(false);
    }
  }

  getNotificationIcon(type: string): string {
    switch (type) {
      case 'order_created': return 'bi-bag-check-fill';
      case 'order_status': return 'bi-truck';
      default: return 'bi-bell-fill';
    }
  }

  getTitle(notif: Notification): string {
    return this.langService.currentLang() === 'en' && notif.titleEn
      ? notif.titleEn
      : notif.title;
  }

  getMessage(notif: Notification): string {
    return this.langService.currentLang() === 'en' && notif.messageEn
      ? notif.messageEn
      : notif.message;
  }

  timeAgo(dateStr: string): string {
    const isEn = this.langService.currentLang() === 'en';
    const now = new Date();
    const date = new Date(dateStr);
    const diffMs = now.getTime() - date.getTime();
    const diffMin = Math.floor(diffMs / 60000);
    const diffHr = Math.floor(diffMin / 60);
    const diffDay = Math.floor(diffHr / 24);

    if (diffMin < 1) return isEn ? 'Just now' : 'Az önce';
    if (diffMin < 60) return isEn ? `${diffMin}m ago` : `${diffMin} dk önce`;
    if (diffHr < 24) return isEn ? `${diffHr}h ago` : `${diffHr} saat önce`;
    if (diffDay < 7) return isEn ? `${diffDay}d ago` : `${diffDay} gün önce`;
    return date.toLocaleDateString();
  }
}
