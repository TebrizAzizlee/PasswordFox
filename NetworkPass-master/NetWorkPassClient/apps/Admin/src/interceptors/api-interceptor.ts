import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../environments/environment';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {

  if (req.url.startsWith('/pass/')) {

    const newUrl = req.url.replace(
      '/pass/',
      environment.apiBaseUrl + '/'
    );

    return next(req.clone({
      url: newUrl
    }));
  }

  return next(req);
};