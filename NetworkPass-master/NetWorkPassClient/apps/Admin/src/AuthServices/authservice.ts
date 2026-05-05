import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, finalize, of, switchMap, tap, throwError } from 'rxjs';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private http = inject(HttpClient);

  // 🔥 STATE
  user = signal<User | null>(null);
  isAuthenticated = signal<boolean>(false);
  isLoading = signal<boolean>(false);

  isInitialized$ = new BehaviorSubject(false);

  // 🔐 LOGIN
  login(data: { loginIdentifier: string; password: string }) {
    this.isLoading.set(true);

    return this.http.post<User>('/pass/auth/login', data, {
      withCredentials: true
    }).pipe(
      switchMap((res) => {
        if (res.requiresTfa) {
          return of(res);
        }

        return this.fetchMe();
      }),
      finalize(() => this.isLoading.set(false)),
      catchError((err) => {
        this.resetAuthState();
        return throwError(() => err);
      })
    );
  }

  // 🔐 LOGIN WITH TFA
  loginWithTfa(data: { userName: string; tfaCode: string }) {
    this.isLoading.set(true);

    return this.http.post('/pass/auth/login-with-tfa', data, {
      withCredentials: true
    }).pipe(
      switchMap(() => this.fetchMe()),
      finalize(() => this.isLoading.set(false)),
      catchError((err) => {
        this.resetAuthState();
        return throwError(() => err);
      })
    );
  }

  // 👤 USER CHECK (CORE FUNCTION)
  fetchMe() {
    return this.http.get<User>('/pass/auth/me', {
      withCredentials: true
    }).pipe(
      tap((user) => {
        this.user.set(user);
        this.isAuthenticated.set(!!user);
      }),
      catchError(() => {
        this.resetAuthState();
        return of(null);
      }),
      finalize(() => {
        this.isInitialized$.next(true);
      })
    );
  }

  // 🔄 REFRESH
  refresh() {
    return this.http.post('/pass/auth/refresh-token', {}, {
      withCredentials: true
    });
  }

  // 🚪 LOGOUT
  logout() {
    return this.http.post('/pass/auth/logout', {}, {
      withCredentials: true
    }).pipe(
      tap(() => this.resetAuthState())
    );
  }

  // 🔧 PRIVATE HELPER
  private resetAuthState() {
    this.user.set(null);
    this.isAuthenticated.set(false);
  }
}