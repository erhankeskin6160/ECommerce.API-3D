import { Component, OnInit, OnDestroy, inject, signal, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { AdminService, DashboardStatsDto } from '../../../services/admin.service';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './admin-dashboard.html'
})
export class AdminDashboard implements OnInit, OnDestroy {
  @ViewChild('revenueChart', { static: false }) revenueChartRef!: ElementRef<HTMLCanvasElement>;

  private adminService = inject(AdminService);
  private chart: Chart | null = null;

  stats = signal<DashboardStatsDto | null>(null);
  lowStockProducts = signal<any[]>([]);
  loading = signal(true);
  error = signal('');

  ngOnInit(): void {
    this.loadStats();
    this.loadLowStock();
  }

  loadStats(): void {
    this.loading.set(true);
    this.adminService.getDashboardStats().subscribe({
      next: (data: DashboardStatsDto) => {
        this.stats.set(data);
        this.loading.set(false);
        setTimeout(() => this.renderChart(data), 80);
      },
      error: () => {
        this.error.set('İstatistikler yüklenirken bir hata oluştu.');
        this.loading.set(false);
      }
    });
  }

  loadLowStock(): void {
    this.adminService.getLowStockProducts().subscribe({
      next: (products) => this.lowStockProducts.set(products),
      error: () => console.error('Low stock products could not be loaded')
    });
  }

  private renderChart(data: DashboardStatsDto): void {
    if (!this.revenueChartRef) return;
    if (this.chart) { this.chart.destroy(); this.chart = null; }

    const ctx = this.revenueChartRef.nativeElement.getContext('2d');
    if (!ctx) return;

    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: ['Günlük', 'Haftalık', 'Aylık'],
        datasets: [
          {
            label: 'Gelir ($)',
            data: [data.dailyRevenue, data.weeklyRevenue, data.monthlyRevenue],
            backgroundColor: [
              'rgba(46, 204, 113, 0.75)',
              'rgba(52, 152, 219, 0.75)',
              'rgba(245, 166, 35, 0.75)'
            ],
            borderColor: [
              'rgba(46, 204, 113, 1)',
              'rgba(52, 152, 219, 1)',
              'rgba(245, 166, 35, 1)'
            ],
            borderWidth: 2,
            borderRadius: 8,
            borderSkipped: false,
          },
          {
            label: 'Sipariş Sayısı',
            data: [data.dailyOrdersCount, data.weeklyOrdersCount, data.monthlyOrdersCount],
            backgroundColor: [
              'rgba(46, 204, 113, 0.2)',
              'rgba(52, 152, 219, 0.2)',
              'rgba(245, 166, 35, 0.2)'
            ],
            borderColor: [
              'rgba(46, 204, 113, 0.5)',
              'rgba(52, 152, 219, 0.5)',
              'rgba(245, 166, 35, 0.5)'
            ],
            borderWidth: 1.5,
            borderRadius: 8,
            borderSkipped: false,
            yAxisID: 'y1'
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'top',
            labels: {
              font: { family: 'Inter', size: 12 },
              padding: 16,
              usePointStyle: true,
              pointStyle: 'circle'
            }
          },
          tooltip: {
            backgroundColor: 'rgba(26,26,46,0.95)',
            titleFont: { family: 'Inter', weight: 'bold' },
            bodyFont: { family: 'Inter' },
            padding: 12,
            cornerRadius: 10,
            callbacks: {
              label: (ctx) => {
                const label = ctx.dataset.label || '';
                const value = ctx.parsed.y;
                return label.includes('Gelir') ? ` ${label}: $${value}` : ` ${label}: ${value}`;
              }
            }
          }
        },
        scales: {
          x: {
            grid: { display: false },
            ticks: { font: { family: 'Inter', weight: 'bold' } }
          },
          y: {
            position: 'left',
            grid: { color: 'rgba(0,0,0,0.04)' },
            ticks: {
              font: { family: 'Inter' },
              callback: (v) => '$' + v
            }
          },
          y1: {
            position: 'right',
            grid: { drawOnChartArea: false },
            ticks: {
              font: { family: 'Inter' },
              callback: (v) => v + ' sipariş'
            }
          }
        }
      }
    });
  }

  ngOnDestroy(): void {
    if (this.chart) { this.chart.destroy(); }
  }
}
