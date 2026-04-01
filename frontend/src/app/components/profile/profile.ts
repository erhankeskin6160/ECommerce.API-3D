import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class Profile implements OnInit {
  private authService = inject(AuthService);

  currentUser = this.authService.currentUser;

  // Profile form
  profileForm = { fullName: '' };
  profileLoading = signal(false);
  profileSuccess = signal('');
  profileError = signal('');

  // Password form
  passwordForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
  passwordLoading = signal(false);
  passwordSuccess = signal('');
  passwordError = signal('');

  // Sidebar active tab
  activeTab = signal<'profile' | 'password'>('profile');

  ngOnInit() {
    this.authService.getProfile().subscribe({
      next: (data) => {
        this.profileForm.fullName = data.fullName;
      },
      error: () => {
        this.profileError.set('Profil bilgileri yüklenemedi.');
      }
    });
  }

  saveProfile() {
    if (!this.profileForm.fullName.trim()) {
      this.profileError.set('Ad Soyad boş olamaz.');
      return;
    }
    this.profileLoading.set(true);
    this.profileError.set('');
    this.profileSuccess.set('');
    this.authService.updateProfile({ fullName: this.profileForm.fullName }).subscribe({
      next: (res) => {
        this.profileSuccess.set(res.message);
        this.profileLoading.set(false);
        setTimeout(() => this.profileSuccess.set(''), 3500);
      },
      error: (err) => {
        this.profileError.set(err.error?.message || 'Profil güncellenemedi.');
        this.profileLoading.set(false);
      }
    });
  }

  changePassword() {
    this.passwordError.set('');
    this.passwordSuccess.set('');
    if (!this.passwordForm.currentPassword || !this.passwordForm.newPassword) {
      this.passwordError.set('Lütfen tüm alanları doldurun.');
      return;
    }
    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.passwordError.set('Yeni şifreler birbiriyle uyuşmuyor.');
      return;
    }
    if (this.passwordForm.newPassword.length < 6) {
      this.passwordError.set('Yeni şifre en az 6 karakter olmalıdır.');
      return;
    }
    this.passwordLoading.set(true);
    this.authService.changePassword({
      currentPassword: this.passwordForm.currentPassword,
      newPassword: this.passwordForm.newPassword
    }).subscribe({
      next: (res) => {
        this.passwordSuccess.set(res.message);
        this.passwordForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
        this.passwordLoading.set(false);
        setTimeout(() => this.passwordSuccess.set(''), 3500);
      },
      error: (err) => {
        this.passwordError.set(err.error?.message || 'Şifre değiştirilemedi.');
        this.passwordLoading.set(false);
      }
    });
  }
}
