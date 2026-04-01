import { Component, OnInit, inject, ViewChild, ElementRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { CartService } from '../../services/cart.service';
import { OrderService, CreateOrderDto } from '../../services/order.service';
import { loadStripe, Stripe, StripeElements, StripeCardElement } from '@stripe/stripe-js';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss'
})
export class Checkout implements OnInit {
  @ViewChild('cardInfo') cardInfo!: ElementRef;
  
  cartService = inject(CartService);
  private orderService = inject(OrderService);
  private http = inject(HttpClient);
  private router = inject(Router);

  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  card: StripeCardElement | null = null;

  loading = signal(false);
  error = signal('');
  success = signal(false);

  selectedPaymentMethod = signal<'card' | 'paypal'>('card');
  paypalLoaded = signal(false);

  // Form Fields
  shippingDetails = {
    fullName: '',
    address: '',
    city: '',
    zipCode: ''
  };

  async ngOnInit() {
    if (this.cartService.items().length === 0) {
      this.router.navigate(['/cart']);
      return;
    }
    
    // Yükleyeceğiniz Public Anahtarınızı (Stripe Dashboard'dan alınan) buraya koyun.
    this.stripe = await loadStripe('pk_test_51IG9ZPKnIIrdOH3NDHBfzaxUGNoFbSesbWHDBsYur8RWA99Lxn4cK9HQunqHy5jU6UEgRtbPxd5FjVTg07Y2Reev00EQEbCAm3'); 
    this.setupStripeElements();
    this.loadPayPalScript();
  }

  selectPaymentMethod(method: 'card' | 'paypal') {
    this.selectedPaymentMethod.set(method);
    if (method === 'paypal' && !this.paypalLoaded()) {
      setTimeout(() => this.initPayPal(), 100);
    }
  }

  loadPayPalScript() {
    if (document.getElementById('paypal-sdk')) return;
    const script = document.createElement('script');
    script.id = 'paypal-sdk';
    script.src = 'https://www.paypal.com/sdk/js?client-id=test&currency=USD';
    script.onload = () => this.paypalLoaded.set(true);
    document.body.appendChild(script);
  }

  initPayPal() {
    if ((window as any).paypal && this.paypalLoaded()) {
      const container = document.getElementById('paypal-button-container');
      if (container) container.innerHTML = ''; // Temizle
      
      (window as any).paypal.Buttons({
        createOrder: (data: any, actions: any) => {
          return actions.order.create({
            purchase_units: [{
              amount: {
                value: this.cartService.totalPrice().toString()
              }
            }]
          });
        },
        onApprove: (data: any, actions: any) => {
          return actions.order.capture().then((details: any) => {
             this.processSuccessfulOrder();
          });
        },
        onError: (err: any) => {
          this.error.set('PayPal işlemi sırasında bir hata oluştu veya işlem iptal edildi.');
        }
      }).render('#paypal-button-container');
    }
  }

  setupStripeElements() {
    if (this.stripe) {
      this.elements = this.stripe.elements();
      
      const style = {
        base: {
          color: '#32325d',
          fontFamily: '"Inter", "Helvetica Neue", Helvetica, sans-serif',
          fontSmoothing: 'antialiased',
          fontSize: '16px',
          '::placeholder': {
            color: '#aab7c4'
          }
        },
        invalid: {
          color: '#fa755a',
          iconColor: '#fa755a'
        }
      };

      this.card = this.elements.create('card', { style: style, hidePostalCode: true });
      this.card.mount(this.cardInfo.nativeElement);

      this.card.on('change', (event: any) => {
        if (event.error) {
          this.error.set(event.error.message);
        } else {
          this.error.set('');
        }
      });
    }
  }

  async handlePayment() {
    if (!this.stripe || !this.card) return;

    if (!this.shippingDetails.fullName || !this.shippingDetails.address || !this.shippingDetails.city) {
      this.error.set('Lütfen tüm teslimat bilgilerini eksiksiz doldurun.');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const totalAmount = this.cartService.totalPrice();
    // Stripe expects amounts in cents
    const amountInCents = Math.round(totalAmount * 100);

    try {
      // 1. Create Payment Intent on the Backend
      console.log('Backend payment intent isteği başlatılıyor, Amount (Cents):', amountInCents);
      const response = await this.http.post<{ clientSecret: string }>(
        'http://localhost:5243/api/payments/create-payment-intent', 
        { amount: amountInCents }
      ).toPromise();
      
      if (!response?.clientSecret) {
        throw new Error('Ödeme isteği başlatılamadı.');
      }

      // 2. Confirm the Payment with Stripe.js
      const result = await this.stripe.confirmCardPayment(response.clientSecret, {
        payment_method: {
          card: this.card,
          billing_details: {
            name: this.shippingDetails.fullName,
            address: {
              line1: this.shippingDetails.address,
              city: this.shippingDetails.city,
              postal_code: this.shippingDetails.zipCode,
            }
          }
        }
      });

      if (result.error) {
        this.error.set(result.error.message || 'Ödeme reddedildi.');
      } else {
        if (result.paymentIntent?.status === 'succeeded') {
           this.processSuccessfulOrder();
        }
      }
    } catch (err: any) {
      // Backend'den (Stripe'dan) gelen detaylı hata mesajını yakala
      const backendError = err.error?.error?.message || err.error?.message;
      this.error.set(backendError || 'Ödeme işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      this.loading.set(false);
    }
  }

  private processSuccessfulOrder() {
    this.success.set(true);
    
    // Create the shipping address string
    const fullAddress = `${this.shippingDetails.address}, ${this.shippingDetails.city}, ${this.shippingDetails.zipCode}`;
    
    // Map cart items to CreateOrderItem DTO
    const orderItems = this.cartService.items().map(item => ({
      productId: item.product.id,
      quantity: item.quantity,
      unitPrice: item.product.price
    }));

    const orderDto: CreateOrderDto = {
      shippingAddress: fullAddress,
      items: orderItems
    };

    this.orderService.createOrder(orderDto).subscribe({
      next: (res) => {
        setTimeout(() => {
          this.cartService.clearCart();
          this.router.navigate(['/orders']); // Go to user's orders page
        }, 2500);
      },
      error: (err) => {
        this.error.set('Ödeme alındı ancak sipariş oluşturulurken bir hata meydana geldi. İletişime geçiniz.');
        this.success.set(false);
      }
    });
  }
}
