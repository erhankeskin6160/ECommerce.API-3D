import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterModule, TranslateModule],
  templateUrl: './cart.html',
  styleUrl: './cart.scss'
})
export class Cart {
  cartService = inject(CartService);
  authService = inject(AuthService);
  private orderService = inject(OrderService);
  private router = inject(Router);

  orderLoading = signal(false);
  orderError = signal('');
  orderSuccess = signal('');


  placeOrder(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    if (this.cartService.items().length > 0) {
      this.router.navigate(['/checkout']);
    }
  }
}
