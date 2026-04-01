import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { WishlistService, WishlistItemResponse } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './wishlist.html',
  styleUrl: './wishlist.scss'
})
export class Wishlist implements OnInit {
  wishlistService = inject(WishlistService);
  cartService = inject(CartService);

  items = signal<WishlistItemResponse[]>([]);
  loading = signal(true);
  addedToCartId = signal<number | null>(null);

  ngOnInit(): void {
    this.wishlistService.getWishlist().subscribe({
      next: (data) => { this.items.set(data); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  removeFromWishlist(productId: number): void {
    this.wishlistService.toggle(productId);
    this.items.update(list => list.filter(i => i.productId !== productId));
  }

  addToCart(item: WishlistItemResponse): void {
    if (!item.product) return;
    this.cartService.addToCart({
      id: item.product.id,
      name: item.product.name,
      description: item.product.description,
      price: item.product.price,
      imageUrl: item.product.imageUrl,
      category: item.product.category,
      stockQuantity: item.product.stockQuantity,
      nameTr: item.product.nameTr || '',
      descriptionTr: item.product.descriptionTr || '',
      additionalImages: [],
      createdAt: item.product.createdAt
    });
    this.addedToCartId.set(item.product.id);
    setTimeout(() => this.addedToCartId.set(null), 1500);
  }
}
