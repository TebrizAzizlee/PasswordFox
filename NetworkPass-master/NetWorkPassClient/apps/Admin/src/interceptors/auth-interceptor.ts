import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError, BehaviorSubject, filter, take } from 'rxjs';
import { AuthService } from '../AuthServices/authservice';
import { environment } from '../environments/environment';

let isRefreshing = false;
const refreshSubject = new BehaviorSubject<boolean>(false);

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const auth = inject(AuthService);

  // 🔴 1. API filter (MƏCBURİ)
  if (!req.url.startsWith(environment.apiBaseUrl)) {
    return next(req);
  }

  // 🔴 2. Auth endpointləri skip et
  if (
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/refresh-token') ||
    req.url.includes('/auth/logout')
  ) {
    return next(req);
  }

  return next(req).pipe(
    catchError((err) => {

      if (err.status !== 401) {
        return throwError(() => err);
      }

      // 🔴 3. Əgər refresh gedirsə → gözlə
      if (isRefreshing) {
        return refreshSubject.pipe(
          filter(v => v === true),
          take(1),
          switchMap(() => next(req))
        );
      }

      // 🔴 4. Refresh başlat
      isRefreshing = true;
      refreshSubject.next(false);

      return auth.refresh().pipe(
        switchMap(() => {

          isRefreshing = false;
          refreshSubject.next(true);

          return next(req);
        }),
        catchError((refreshErr) => {

          isRefreshing = false;
          refreshSubject.next(false);

          auth.logout();

          return throwError(() => refreshErr);
        })
      );
    })
  );
};