import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './verify-email.html'
})
export class VerifyEmail implements OnInit {
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);

  status = signal<'loading' | 'success' | 'error'>('loading');
  message = signal('E-posta adresiniz doğrulanıyor, lütfen bekleyin...');

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'];
      const token = params['token'];

      if (userId && token) {
        this.verifyAccount(userId, token);
      } else {
        this.status.set('error');
        this.message.set('Geçersiz doğrulama bağlantısı. Link eksik veya hatalı olabilir.');
      }
    });
  }

  private verifyAccount(userId: string, token: string): void {
    this.authService.verifyEmail(userId, token).subscribe({
      next: (res) => {
        this.status.set('success');
        this.message.set(res.message || 'E-posta adresiniz başarıyla doğrulandı!');
      },
      error: (err) => {
        this.status.set('error');
        this.message.set(err.error?.message || 'Doğrulama işlemi başarısız oldu. Linkin süresi dolmuş olabilir.');
      }
    });
  }
}
