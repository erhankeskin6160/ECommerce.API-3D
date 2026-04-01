import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
  private authService = inject(AuthService);
  private router = inject(Router);

  name = signal('');
  email = signal('');
  password = signal('');
  confirmPassword = signal('');
  loading = signal(false);
  error = signal('');
  showPassword = signal(false);

  onSubmit(): void {
    if (!this.name() || !this.email() || !this.password() || !this.confirmPassword()) {
      this.error.set('Lütfen tüm alanları doldurun.');
      return;
    }
    if (this.password() !== this.confirmPassword()) {
      this.error.set('Şifreler eşleşmiyor.');
      return;
    }
    if (this.password().length < 6) {
      this.error.set('Şifre en az 6 karakter olmalıdır.');
      return;
    }
    this.error.set('');
    this.loading.set(true);

    this.authService.register({
      fullName: this.name(),
      email: this.email(),
      password: this.password()
    }).subscribe({
      next: () => {
        this.loading.set(false);
        // Auto-login after register
        this.authService.login({ email: this.email(), password: this.password() }).subscribe({
          next: () => this.router.navigate(['/']),
          error: () => this.router.navigate(['/login'])
        });
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message ?? 'Kayıt başarısız.');
      }
    });
  }
}
