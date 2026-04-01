import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product';
import { CartService } from '../../services/cart.service';
import { StarRating } from '../star-rating/star-rating';
import { Review } from '../../models/review';
import { ReviewService } from '../../services/review.service';
import { StockNotificationService } from '../../services/stock-notification.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule, StarRating],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss'
})
export class ProductDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  cartService = inject(CartService);
  translateService = inject(TranslateService);
  reviewService = inject(ReviewService);
  stockService = inject(StockNotificationService);
  authService = inject(AuthService);

  readonly baseUrl = 'http://localhost:5243';

  product = signal<Product | null>(null);
  selectedImage = signal<string>('');
  loading = signal(true);
  error = signal('');
  addedToCart = signal(false);
  relatedProducts = signal<Product[]>([]);
  reviews = signal<Review[]>([]);
  averageRating = signal(0);
  isSubscribedToStock = signal(false);
  isSubscribing = signal(false);

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      this.loading.set(true);
      this.error.set('');
      
      this.productService.getProduct(id).subscribe({
        next: (p) => { 
          this.product.set(p); 
          this.selectedImage.set(p.imageUrl || '');
          this.loading.set(false);
          window.scrollTo(0, 0); // Scroll to top on navigation

          // Load stock subscription status if authenticated and out of stock
          if (p.stockQuantity === 0 && this.authService.isLoggedIn()) {
            this.stockService.checkSubscriptionStatus(p.id).subscribe({
              next: (res) => this.isSubscribedToStock.set(res.isSubscribed),
              error: () => this.isSubscribedToStock.set(false)
            });
          }

          // Load related products
          this.productService.getProducts().subscribe({
            next: (rp) => {
              // Filter out current product
              const otherProducts = rp.filter(x => x.id !== p.id);
              
              // Try to find same category first
              let related = otherProducts.filter(x => x.category === p.category);
              
              // If not enough products in the same category, add from others
              if (related.length < 4) {
                const remaining = otherProducts.filter(x => x.category !== p.category);
                related = [...related, ...remaining];
              }
              
              // Take up to 4 items
              this.relatedProducts.set(related.slice(0, 4));
            }
          });

          // Load reviews
          this.reviewService.getReviews(id).subscribe({
            next: (revs) => {
              this.reviews.set(revs);
              this.calculateAverage(revs);
            }
          });
        },
        error: (err) => { 
          this.error.set('Ürün bulunamadı: ' + (err.message || err)); 
          this.loading.set(false); 
        }
      });
    });
  }

  getLocalizedName(): string {
    const p = this.product();
    if (!p) return '';
    const lang = this.translateService.currentLang || this.translateService.getDefaultLang() || 'tr';
    return (lang === 'tr' && p.nameTr) ? p.nameTr : p.name;
  }

  getLocalizedDescription(): string {
    const p = this.product();
    if (!p) return '';
    const lang = this.translateService.currentLang || this.translateService.getDefaultLang() || 'tr';
    return (lang === 'tr' && p.descriptionTr) ? p.descriptionTr : p.description;
  }

  getAllImages(): string[] {
    const p = this.product();
    if (!p) return [];
    const images: string[] = [];
    if (p.imageUrl) images.push(p.imageUrl);
    if (p.additionalImages && p.additionalImages.length > 0) {
      images.push(...p.additionalImages);
    }
    return images;
  }

  prevImage(event?: Event) {
    if (event) event.stopPropagation();
    const all = this.getAllImages();
    if (all.length <= 1) return;
    let idx = all.indexOf(this.selectedImage());
    idx = idx <= 0 ? all.length - 1 : idx - 1;
    this.selectedImage.set(all[idx]);
  }

  nextImage(event?: Event) {
    if (event) event.stopPropagation();
    const all = this.getAllImages();
    if (all.length <= 1) return;
    let idx = all.indexOf(this.selectedImage());
    idx = idx >= all.length - 1 ? 0 : idx + 1;
    this.selectedImage.set(all[idx]);
  }

  selectImage(url: string) {
    this.selectedImage.set(url);
  }

  addedToCartId = signal<number | null>(null);

  addToCart(product?: Product): void {
    const p = product || this.product();
    if (!p) return;
    this.cartService.addToCart(p);
    
    if (product) {
      this.addedToCartId.set(p.id);
      setTimeout(() => this.addedToCartId.set(null), 1500);
    } else {
      this.addedToCart.set(true);
      setTimeout(() => this.addedToCart.set(false), 2000);
    }
  }

  subscribeToStock() {
    const p = this.product();
    if (!p) return;
    
    if (!this.authService.isLoggedIn()) {
      alert(this.translateService.instant('PRODUCT_DETAIL.LOGIN_REQUIRED') || 'Lütfen giriş yapın.');
      return;
    }

    this.isSubscribing.set(true);
    this.stockService.subscribeToStock(p.id).subscribe({
      next: () => {
        this.isSubscribedToStock.set(true);
        this.isSubscribing.set(false);
      },
      error: () => {
        alert(this.translateService.instant('PRODUCT_DETAIL.SUBSCRIBE_ERROR') || 'Bir hata oluştu.');
        this.isSubscribing.set(false);
      }
    });
  }

  calculateAverage(revs: Review[]) {
    if (revs.length === 0) {
      this.averageRating.set(0);
    } else {
      const sum = revs.reduce((a, b) => a + b.rating, 0);
      this.averageRating.set(Math.round((sum / revs.length) * 10) / 10);
    }
  }
}
