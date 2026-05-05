import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { appRoutes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptors/http-interceptor';
import { credentialsInterceptor } from './interceptors/credentials-interceptor';
import { apiInterceptor } from './interceptors/api-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
   provideRouter(appRoutes),
   provideHttpClient(withInterceptors([apiInterceptor,credentialsInterceptor,authInterceptor]))
  ],
};
