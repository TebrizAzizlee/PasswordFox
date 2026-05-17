
import {
  HttpInterceptorFn
} from '@angular/common/http';

function getCookie(
  name: string
): string | null {

  const value =
    `; ${document.cookie}`;

  const parts =
    value.split(`; ${name}=`);

  if (parts.length === 2) {

    return parts
      .pop()
      ?.split(';')
      .shift() ?? null;
  }

  return null;
}

function shouldAttachCsrf(
  method: string
): boolean {

  return (
    method === 'POST' ||
    method === 'PUT' ||
    method === 'PATCH' ||
    method === 'DELETE'
  );
}

export const csrfInterceptor:
HttpInterceptorFn = (req, next) => {

  // 🔥 yalnız state-changing request-lər
  if (!shouldAttachCsrf(req.method)) {
    return next(req);
  }

  const csrf =
    getCookie('X-CSRF-TOKEN');

  // 🔥 csrf yoxdursa davam et
  if (!csrf) {
    return next(req);
  }

  const request = req.clone({
    setHeaders: {
      'X-CSRF-TOKEN': csrf
    }
  });

  return next(request);
}
