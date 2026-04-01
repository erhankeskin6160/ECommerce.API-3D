import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/product-list/product-list').then(m => m.ProductList)
  },
  {
    path: 'products/:id',
    loadComponent: () =>
      import('./components/product-detail/product-detail').then(m => m.ProductDetail)
  },
  {
    path: 'cart',
    loadComponent: () =>
      import('./components/cart/cart').then(m => m.Cart)
  },
  {
    path: 'orders',
    loadComponent: () =>
      import('./components/orders/orders').then(m => m.Orders),
    canActivate: [authGuard]
  },
  {
    path: 'checkout',
    loadComponent: () =>
      import('./components/checkout/checkout').then(m => m.Checkout),
    canActivate: [authGuard]
  },
  {
    path: 'profile',
    loadComponent: () =>
      import('./components/profile/profile').then(m => m.Profile),
    canActivate: [authGuard]
  },
  {
    path: 'wishlist',
    loadComponent: () =>
      import('./components/wishlist/wishlist').then(m => m.Wishlist),
    canActivate: [authGuard]
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./components/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./components/register/register').then(m => m.Register)
  },
  {
    path: 'admin-login',
    loadComponent: () =>
      import('./components/admin/admin-login/admin-login').then(m => m.AdminLogin)
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./components/admin/admin-layout/admin-layout').then(m => m.AdminLayout),
    canActivate: [adminGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./components/admin/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard)
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./components/admin/admin-orders/admin-orders').then(m => m.AdminOrders)
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./components/admin/admin-products/admin-products').then(m => m.AdminProducts)
      }
    ]
  },
  {
    path: 'verify-email',
    loadComponent: () =>
      import('./components/verify-email/verify-email').then(m => m.VerifyEmail)
  },
  { path: '**', redirectTo: '' }
];
