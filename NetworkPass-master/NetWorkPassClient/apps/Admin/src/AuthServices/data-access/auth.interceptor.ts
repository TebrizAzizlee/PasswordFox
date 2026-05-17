
import {
  HttpInterceptorFn
} from '@angular/common/http';

import { inject } from '@angular/core';

import { AuthStore } from './auth.store';

function shouldSkipAuth(
  url: string
): boolean {

  return (
    url.includes('/auth/login') ||
    url.includes('/auth/refresh-token')
  );
}

export const authInterceptor:
HttpInterceptorFn = (req, next) => {

  const store =
    inject(AuthStore);

  // 🔥 cookie request üçün
  let request = req.clone({
    withCredentials: true
  });

  // 🔥 login / refresh bypass
  if (shouldSkipAuth(req.url)) {
    return next(request);
  }

  const accessToken =
    store.accessToken();

  // 🔥 token yoxdursa davam et
  if (!accessToken) {
    return next(request);
  }

  // 🔥 bearer əlavə et
  request = request.clone({
    setHeaders: {
      Authorization:
        `Bearer ${accessToken}`
    }
  });

  return next(request);
};

