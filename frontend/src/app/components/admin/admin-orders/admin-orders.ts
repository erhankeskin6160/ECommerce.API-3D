import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { AdminService, AdminOrderDto } from '../../../services/admin.service';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './admin-orders.html'
})
export class AdminOrders implements OnInit {
  private readonly adminService = inject(AdminService);

  allOrders = signal<AdminOrderDto[]>([]);
  loading = signal(true);
  error = signal('');
  toast = signal<{ message: string; type: 'success' | 'danger' } | null>(null);

  // Filter state
  searchQuery = signal('');
  selectedStatus = signal('');

  availableStatuses = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];

  filteredOrders = computed(() => {
    const q = this.searchQuery().toLowerCase().trim();
    const st = this.selectedStatus();
    return this.allOrders().filter(o => {
      const matchSearch = !q ||
        o.userFullName.toLowerCase().includes(q) ||
        o.userEmail.toLowerCase().includes(q) ||
        String(o.id).includes(q);
      const matchStatus = !st || o.status === st;
      return matchSearch && matchStatus;
    });
  });

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.loading.set(true);
    this.adminService.getOrders().subscribe({
      next: (data) => {
        this.allOrders.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Siparişler yüklenemedi.');
        this.loading.set(false);
      }
    });
  }

  updateStatus(order: AdminOrderDto, newStatus: string) {
    const previousStatus = order.status;
    this.adminService.updateOrderStatus(order.id, newStatus).subscribe({
      next: (res) => {
        order.status = res.status;
        this.showToast(`Sipariş #${order.id} durumu "${res.status}" olarak güncellendi.`, 'success');
      },
      error: () => {
        order.status = previousStatus;
        this.showToast('Durum güncellenemedi. Lütfen tekrar deneyin.', 'danger');
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Pending': return 'badge bg-warning text-dark';
      case 'Processing': return 'badge bg-info text-white';
      case 'Shipped': return 'badge bg-primary text-white';
      case 'Delivered': return 'badge bg-success text-white';
      case 'Cancelled': return 'badge bg-danger text-white';
      default: return 'badge bg-secondary text-white';
    }
  }

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'Beklemede',
      'Processing': 'Hazırlanıyor',
      'Shipped': 'Kargoda',
      'Delivered': 'Teslim Edildi',
      'Cancelled': 'İptal Edildi'
    };
    return map[status] || status;
  }

  clearFilters() {
    this.searchQuery.set('');
    this.selectedStatus.set('');
  }

  private showToast(message: string, type: 'success' | 'danger') {
    this.toast.set({ message, type });
    setTimeout(() => this.toast.set(null), 3500);
  }

  // --- Shipping Logic ---
  selectedOrderForShipping = signal<AdminOrderDto | null>(null);
  shippingForm = signal({ company: '', trackingNumber: '' });
  
  availableCompanies = ['Yurtiçi Kargo', 'Aras Kargo', 'MNG Kargo', 'Sürat Kargo', 'PTT Kargo', 'UPS', 'Diğer'];

  openShippingModal(order: AdminOrderDto) {
    this.selectedOrderForShipping.set(order);
    this.shippingForm.set({
      company: order.shippingCompany || '',
      trackingNumber: order.trackingNumber || ''
    });
  }

  closeShippingModal() {
    this.selectedOrderForShipping.set(null);
  }

  generateTrackingUrl(company: string, trackingNumber: string): string {
    const num = encodeURIComponent(trackingNumber);
    switch (company) {
      case 'Yurtiçi Kargo': return `https://www.yurticikargo.com/tr/online-servisler/gonderi-sorgula?code=${num}`;
      case 'Aras Kargo': return `https://araskargo.com.tr/kargo-takip?query=${num}`;
      case 'MNG Kargo': return `https://www.mngkargo.com.tr/gonderitakipdetay.aspx?takipno=${num}`;
      case 'Sürat Kargo': return `https://www.suratkargo.com.tr/KargoTakip/?kargotakipno=${num}`;
      case 'PTT Kargo': return `https://gonderitakip.ptt.gov.tr/Track/Verify?q=${num}`;
      case 'UPS': return `https://www.ups.com.tr/gonderi_takip_detay.aspx?tracking_number=${num}`;
      default: return '';
    }
  }

  submitShipping() {
    const order = this.selectedOrderForShipping();
    if (!order) return;
    
    const { company, trackingNumber } = this.shippingForm();
    if (!company || !trackingNumber) {
      this.showToast('Lütfen firma ve takip numarasını doldurun.', 'danger');
      return;
    }

    const trackingUrl = this.generateTrackingUrl(company, trackingNumber);

    this.adminService.updateOrderShipping(order.id, {
      shippingCompany: company,
      trackingNumber,
      trackingUrl
    }).subscribe({
      next: (res) => {
        order.shippingCompany = res.shippingCompany;
        order.trackingNumber = res.trackingNumber;
        order.trackingUrl = res.trackingUrl;
        order.status = res.status; // Might be updated to Shipped by backend
        this.showToast('Kargo bilgileri başarıyla kaydedildi!', 'success');
        this.closeShippingModal();
      },
      error: () => this.showToast('Kargo bilgisi kaydedilirken hata oluştu.', 'danger')
    });
  }
}
