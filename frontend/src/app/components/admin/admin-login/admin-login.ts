import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-login.html',
  styleUrl: './admin-login.scss'
})
export class AdminLogin {
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
        // Token auth.service içinde setleniyor. Şimdi isAdmin mi kontrol edelim.
        if (this.authService.isAdmin()) {
           this.router.navigate(['/admin']);
        } else {
           // Admin değilse, normal login değil bu sayfa. 
           // Yetkisiz uyarısı ver ve logout yap.
           this.authService.logout();
           this.error.set('Yetkisiz Giriş: Bu alana sadece yöneticiler erişebilir.');
        }
      },
      error: (err: any) => {
        this.loading.set(false);
        this.error.set(err.error?.message ?? 'Giriş başarısız. E-posta veya şifre hatalı.');
      }
    });
  }
}
