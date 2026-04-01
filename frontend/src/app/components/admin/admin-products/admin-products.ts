import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { AdminService } from '../../../services/admin.service';
import { ProductService } from '../../../services/product';
import { Product } from '../../../models/product';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './admin-products.html'
})
export class AdminProducts implements OnInit {
  private adminService = inject(AdminService);
  private productService = inject(ProductService);

  products = signal<Product[]>([]);
  loading = signal(true);
  error = signal('');

  // Add/Edit Modal
  showModal = signal(false);
  isEditing = signal(false);
  saving = signal(false);

  // Delete Confirm Modal
  showDeleteModal = signal(false);
  productToDeleteId = signal<number | null>(null);
  productToDeleteName = signal('');
  deleting = signal(false);

  // Toast
  toast = signal<{ message: string; type: 'success' | 'danger' } | null>(null);

  currentProduct: Partial<Product> = {
    name: '',
    description: '',
    price: 0,
    stockQuantity: 0,
    imageUrl: '',
    category: ''
  };

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.loading.set(true);
    this.productService.getProducts().subscribe({
      next: (data: Product[]) => {
        this.products.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Ürünler yüklenemedi.');
        this.loading.set(false);
      }
    });
  }

  openCreateModal() {
    this.isEditing.set(false);
    this.currentProduct = { name: '', description: '', price: 0, stockQuantity: 0, imageUrl: '', category: '' };
    this.showModal.set(true);
  }

  openEditModal(product: Product) {
    this.isEditing.set(true);
    this.currentProduct = { ...product };
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  saveProduct() {
    this.saving.set(true);
    if (this.isEditing() && this.currentProduct.id) {
      this.adminService.updateProduct(this.currentProduct as Product).subscribe({
        next: () => {
          this.loadProducts();
          this.closeModal();
          this.saving.set(false);
          this.showToast('Ürün başarıyla güncellendi.', 'success');
        },
        error: () => {
          this.saving.set(false);
          this.showToast('Güncelleme başarısız oldu.', 'danger');
        }
      });
    } else {
      this.adminService.createProduct(this.currentProduct as Omit<Product, 'id'>).subscribe({
        next: () => {
          this.loadProducts();
          this.closeModal();
          this.saving.set(false);
          this.showToast('Ürün başarıyla eklendi.', 'success');
        },
        error: () => {
          this.saving.set(false);
          this.showToast('Ürün eklenemedi.', 'danger');
        }
      });
    }
  }

  confirmDelete(product: Product) {
    this.productToDeleteId.set(product.id);
    this.productToDeleteName.set(product.name);
    this.showDeleteModal.set(true);
  }

  cancelDelete() {
    this.showDeleteModal.set(false);
    this.productToDeleteId.set(null);
    this.productToDeleteName.set('');
  }

  executeDelete() {
    const id = this.productToDeleteId();
    if (id === null) return;
    this.deleting.set(true);
    this.adminService.deleteProduct(id).subscribe({
      next: () => {
        this.loadProducts();
        this.cancelDelete();
        this.deleting.set(false);
        this.showToast('Ürün başarıyla silindi.', 'success');
      },
      error: () => {
        this.deleting.set(false);
        this.cancelDelete();
        this.showToast('Ürün silinemedi.', 'danger');
      }
    });
  }

  private showToast(message: string, type: 'success' | 'danger') {
    this.toast.set({ message, type });
    setTimeout(() => this.toast.set(null), 3500);
  }
}
