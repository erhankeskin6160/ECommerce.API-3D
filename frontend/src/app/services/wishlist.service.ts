import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

export interface WishlistProduct {
  id: number;
  name: string;
  nameTr: string;
  description: string;
  descriptionTr: string;
  price: number;
  imageUrl: string;
  category: string;
  stockQuantity: number;
  createdAt: string;
}

export interface WishlistItemResponse {
  id: number;
  productId: number;
  createdAt: string;
  product: WishlistProduct;
}

@Injectable({ providedIn: 'root' })
export class WishlistService {
  private readonly apiUrl = 'http://localhost:5243/api/wishlist';
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  wishlistCount = signal<number>(0);
  favoritedProductIds = signal<Set<number>>(new Set());

  fetchCount(): void {
    if (!this.authService.isLoggedIn()) return;
    this.http.get<{ count: number }>(`${this.apiUrl}/count`).subscribe({
      next: (data) => this.wishlistCount.set(data.count),
      error: () => {}
    });
  }

  fetchFavoritedIds(): void {
    if (!this.authService.isLoggedIn()) return;
    this.http.get<number[]>(`${this.apiUrl}/product-ids`).subscribe({
      next: (ids) => this.favoritedProductIds.set(new Set(ids)),
      error: () => {}
    });
  }

  isFavorited(productId: number): boolean {
    return this.favoritedProductIds().has(productId);
  }

  toggle(productId: number): void {
    if (!this.authService.isLoggedIn()) return;

    if (this.isFavorited(productId)) {
      this.http.delete(`${this.apiUrl}/${productId}`).subscribe({
        next: () => {
          this.favoritedProductIds.update(set => {
            const newSet = new Set(set);
            newSet.delete(productId);
            return newSet;
          });
          this.wishlistCount.update(c => Math.max(0, c - 1));
        }
      });
    } else {
      this.http.post(`${this.apiUrl}/${productId}`, {}).subscribe({
        next: () => {
          this.favoritedProductIds.update(set => {
            const newSet = new Set(set);
            newSet.add(productId);
            return newSet;
          });
          this.wishlistCount.update(c => c + 1);
        }
      });
    }
  }

  getWishlist() {
    return this.http.get<WishlistItemResponse[]>(this.apiUrl);
  }
}
