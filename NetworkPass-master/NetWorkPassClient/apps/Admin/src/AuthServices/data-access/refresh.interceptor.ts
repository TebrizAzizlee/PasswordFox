import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';

import { inject } from '@angular/core';

import {
  BehaviorSubject,
  catchError,
  filter,
  switchMap,
  take,
  throwError,
} from 'rxjs';

import { AuthService } from './auth.service';

let isRefreshing = false;

const refreshSubject = new BehaviorSubject<string | null>(null);

function shouldSkipRefresh(request: HttpRequest<unknown>): boolean {
  return (
    request.url.includes('/auth/login') ||
    request.url.includes('/auth/refresh-token') ||
    request.url.includes('/auth/logout')
  );
}

function createRetryRequest(
  request: HttpRequest<unknown>,
  accessToken: string,
): HttpRequest<unknown> {
  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`,
      'x-refresh-retry': 'true',
    },
  });
}

export const refreshInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  if (shouldSkipRefresh(req)) {
    return next(req);
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // yalnız 401 handle et
      if (error.status !== 401) {
        return throwError(() => error);
      }

      // retry olunmuş request yenidən refresh etməsin
      if (req.headers.has('x-refresh-retry')) {
        authService.logoutLocal();

        return throwError(() => error);
      }

      // refresh artıq işləyirsə queue gözlə
      if (isRefreshing) {
        return refreshSubject.pipe(
          filter((token): token is string => token !== null),

          take(1),

          switchMap((token) => {
            const retryRequest = createRetryRequest(req, token);

            return next(retryRequest);
          }),
        );
      }

      // refresh başlat
      isRefreshing = true;

      refreshSubject.next(null);

      return authService.refresh().pipe(
        switchMap((accessToken) => {
          isRefreshing = false;

          refreshSubject.next(accessToken);

          const retryRequest = createRetryRequest(req, accessToken);

          return next(retryRequest);
        }),

        catchError((refreshError) => {
          isRefreshing = false;

          authService.logoutLocal();

          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
