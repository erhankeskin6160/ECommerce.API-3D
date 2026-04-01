import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);

  email = signal('');
  password = signal('');
  loading = signal(false);
  error = signal('');
  showPassword = signal(false);

  onSubmit(): void {
    if (!this.email() || !this.password()) {
      this.error.set('Lütfen tüm alanları doldurun.');
      return;
    }
    this.error.set('');
    this.loading.set(true);

    this.authService.login({ email: this.email(), password: this.password() }).subscribe({
      next: () => {
        this.loading.set(false);
        if (this.authService.isAdmin()) {
          // Manually clear tokens to avoid authService.logout() redirecting to '/'
          localStorage.removeItem('auth_user');
          localStorage.removeItem('auth_token');
          // We can't easily access the private _currentUser signal to set it to null, 
          // so we use the logout method but we will immediately redirect to admin-login
          this.authService.logout();
          setTimeout(() => {
            this.router.navigate(['/admin-login']);
          }, 10);
        } else {
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message ?? 'Giriş başarısız. E-posta veya şifre hatalı.');
      }
    });
  }
}
