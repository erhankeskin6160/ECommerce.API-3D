import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

export interface AuthUser {
  email: string;
  fullName: string;
  token: string;
  expiresAt: string;
}

export interface RegisterDto {
  fullName: string;
  email: string;
  password: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface UpdateProfileDto {
  fullName: string;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:5243/api/auth';
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';

  private _currentUser = signal<AuthUser | null>(this.loadUserFromStorage());

  currentUser = this._currentUser.asReadonly();
  isLoggedIn = computed(() => {
    const user = this._currentUser();
    if (!user) return false;
    return new Date(user.expiresAt) > new Date();
  });

  get decodedToken(): any {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = token.split('.')[1];
      const decodedPayload = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(decodedPayload);
    } catch {
      return null;
    }
  }

  isAdmin = computed(() => {
    const user = this._currentUser();
    if (!user) return false;
    const decoded = this.decodedToken;
    if (!decoded) return false;
    const roles = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if (Array.isArray(roles)) {
      return roles.includes('Admin');
    }
    return roles === 'Admin';
  });

  constructor(private http: HttpClient, private router: Router) {}

  register(dto: RegisterDto): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/register`, dto);
  }

  login(dto: LoginDto): Observable<AuthUser> {
    return this.http.post<AuthUser>(`${this.apiUrl}/login`, dto).pipe(
      tap(user => {
        this._currentUser.set(user);
        localStorage.setItem(this.USER_KEY, JSON.stringify(user));
      })
    );
  }

  verifyEmail(userId: string, token: string): Observable<{ message: string }> {
    return this.http.get<{ message: string }>(`${this.apiUrl}/verify-email`, {
      params: { userId, token }
    });
  }

  getProfile(): Observable<{ fullName: string; email: string }> {
    return this.http.get<{ fullName: string; email: string }>(`${this.apiUrl}/profile`);
  }

  updateProfile(dto: UpdateProfileDto): Observable<{ message: string; fullName: string; email: string }> {
    return this.http.put<{ message: string; fullName: string; email: string }>(`${this.apiUrl}/profile`, dto).pipe(
      tap(res => {
        const current = this._currentUser();
        if (current) {
          const updated = { ...current, fullName: res.fullName };
          this._currentUser.set(updated);
          localStorage.setItem(this.USER_KEY, JSON.stringify(updated));
        }
      })
    );
  }

  changePassword(dto: ChangePasswordDto): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/change-password`, dto);
  }

  logout(): void {
    this._currentUser.set(null);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.TOKEN_KEY);
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    return this._currentUser()?.token ?? null;
  }

  private loadUserFromStorage(): AuthUser | null {
    try {
      const raw = localStorage.getItem(this.USER_KEY);
      if (!raw) return null;
      const user: AuthUser = JSON.parse(raw);
      if (new Date(user.expiresAt) < new Date()) {
        localStorage.removeItem(this.USER_KEY);
        return null;
      }
      return user;
    } catch {
      return null;
    }
  }
}
