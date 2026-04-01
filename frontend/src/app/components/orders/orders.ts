import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService, OrderResponse } from '../../services/order.service';
import { ReviewForm } from '../review-form/review-form';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterLink, ReviewForm],
  templateUrl: './orders.html',
  styleUrl: './orders.scss'
})
export class Orders implements OnInit {
  private orderService = inject(OrderService);

  orders = signal<OrderResponse[]>([]);
  loading = signal(true);
  error = signal('');

  // Modal state
  selectedOrder = signal<OrderResponse | null>(null);
  modalVisible = signal(false);

  reviewingProductId = signal<number | null>(null);
  reviewSuccessMsg = signal('');

  ngOnInit(): void {
    this.orderService.getOrders().subscribe({
      next: (data) => { this.orders.set(data); this.loading.set(false); },
      error: () => { this.error.set('Siparişler yüklenemedi.'); this.loading.set(false); }
    });
  }

  openModal(order: OrderResponse): void {
    this.selectedOrder.set(order);
    this.modalVisible.set(true);
    document.body.style.overflow = 'hidden';
  }

  closeModal(): void {
    this.modalVisible.set(false);
    this.reviewingProductId.set(null);
    this.reviewSuccessMsg.set('');
    document.body.style.overflow = '';
    setTimeout(() => this.selectedOrder.set(null), 300);
  }

  openReviewForm(productId: number) {
    this.reviewingProductId.set(productId);
    this.reviewSuccessMsg.set('');
  }

  cancelReview() {
    this.reviewingProductId.set(null);
  }

  onReviewAdded(event: any) {
    this.reviewingProductId.set(null);
    this.reviewSuccessMsg.set('Değerlendirmeniz başarıyla alındı. Teşekkür ederiz!');
    setTimeout(() => this.reviewSuccessMsg.set(''), 5000);
  }

  onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.closeModal();
    }
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'pending':    return 'status-pending';
      case 'processing': return 'status-processing';
      case 'shipped':    return 'status-shipped';
      case 'delivered':  return 'status-delivered';
      case 'cancelled':  return 'status-cancelled';
      default:           return 'status-default';
    }
  }

  getBadgeClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'pending':    return 'bg-warning text-dark';
      case 'processing': return 'bg-info text-white';
      case 'shipped':    return 'bg-primary text-white';
      case 'delivered':  return 'bg-success text-white';
      case 'cancelled':  return 'bg-danger text-white';
      default:           return 'bg-secondary text-white';
    }
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      'pending':    'Beklemede',
      'processing': 'Hazırlanıyor',
      'shipped':    'Kargoda',
      'delivered':  'Teslim Edildi',
      'cancelled':  'İptal Edildi'
    };
    return labels[status.toLowerCase()] ?? status;
  }

  getStatusIcon(status: string): string {
    const icons: Record<string, string> = {
      'pending':    'bi-clock',
      'processing': 'bi-gear',
      'shipped':    'bi-truck',
      'delivered':  'bi-check-circle',
      'cancelled':  'bi-x-circle'
    };
    return icons[status.toLowerCase()] ?? 'bi-circle';
  }
}
