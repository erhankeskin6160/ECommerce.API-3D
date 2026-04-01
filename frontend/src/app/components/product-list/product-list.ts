import { Component, OnInit, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductList implements OnInit {
  private productService = inject(ProductService);
  cartService = inject(CartService);
  wishlistService = inject(WishlistService);
  authService = inject(AuthService);

  constructor() {
    // Reset to page 1 whenever search/sort changes
    effect(() => {
      this.searchQuery();
      this.minPrice();
      this.maxPrice();
      this.sortBy();
      this.currentPage.set(1);
    });
  }

  private allProducts = signal<Product[]>([]);
  categories = signal<string[]>([]);
  loading = signal(true);
  error = signal('');

  // Filter state
  searchQuery = signal('');
  selectedCategory = signal('');
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  addedToCartId = signal<number | null>(null);
  sortBy = signal<string>('recommended');

  // Pagination
  currentPage = signal(1);
  readonly pageSize = 12;

  hasActiveFilter = computed(() =>
    !!this.searchQuery() || !!this.selectedCategory() || this.minPrice() !== null || this.maxPrice() !== null
  );

  filteredProducts = computed(() => {
    const q = this.searchQuery().toLowerCase().trim();
    const cat = this.selectedCategory();
    const min = this.minPrice();
    const max = this.maxPrice();
    const sort = this.sortBy();

    let products = this.allProducts().filter(p => {
      const matchName = !q || p.name.toLowerCase().includes(q) || p.description.toLowerCase().includes(q);
      const matchCat = !cat || p.category === cat;
      const matchMin = min === null || p.price >= min;
      const matchMax = max === null || p.price <= max;
      return matchName && matchCat && matchMin && matchMax;
    });

    // Sorting
    switch (sort) {
      case 'priceLow':
        products.sort((a, b) => a.price - b.price);
        break;
      case 'priceHigh':
        products.sort((a, b) => b.price - a.price);
        break;
      case 'newest':
        products.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        break;
      case 'name':
        products.sort((a, b) => a.name.localeCompare(b.name));
        break;
      case 'rating':
        products.sort((a, b) => this.getAverageRating(b) - this.getAverageRating(a));
        break;
      default:
        break;
    }

    return products;
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.filteredProducts().length / this.pageSize)));

  pagedProducts = computed(() => {
    const page = this.currentPage();
    const size = this.pageSize;
    return this.filteredProducts().slice((page - 1) * size, page * size);
  });

  // Smart page numbers: always show first, last, current ±1, with ellipsis
  pageNumbers = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: (number | '...')[] = [];

    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
      return pages;
    }

    pages.push(1);
    if (current > 3) pages.push('...');
    for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
      pages.push(i);
    }
    if (current < total - 2) pages.push('...');
    pages.push(total);
    return pages;
  });

  startItem = computed(() => (this.currentPage() - 1) * this.pageSize + 1);
  endItem = computed(() => Math.min(this.currentPage() * this.pageSize, this.filteredProducts().length));

  private getAverageRating(product: Product): number {
    if (!product.reviews || product.reviews.length === 0) return 0;
    const total = product.reviews.reduce((acc, r) => acc + r.rating, 0);
    return total / product.reviews.length;
  }

  ngOnInit(): void {
    this.productService.getProducts().subscribe({
      next: (data) => { this.allProducts.set(data); this.loading.set(false); },
      error: (err) => { this.error.set('Ürünler yüklenemedi: ' + (err.message || err)); this.loading.set(false); }
    });
    this.productService.getCategories().subscribe({
      next: (cats) => this.categories.set(cats),
      error: () => {}
    });
    if (this.authService.isLoggedIn()) {
      this.wishlistService.fetchFavoritedIds();
    }
  }

  addToCart(product: Product): void {
    this.cartService.addToCart(product);
    this.addedToCartId.set(product.id);
    setTimeout(() => this.addedToCartId.set(null), 1500);
  }

  selectCategory(cat: string): void {
    this.selectedCategory.set(cat);
    this.currentPage.set(1);
  }

  clearFilters(): void {
    this.searchQuery.set('');
    this.selectedCategory.set('');
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.currentPage.set(1);
  }

  goToPage(page: number | '...'): void {
    if (page === '...' || page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    // Smooth scroll to products section
    document.getElementById('products')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  prevPage(): void { this.goToPage(this.currentPage() - 1); }
  nextPage(): void { this.goToPage(this.currentPage() + 1); }
}

