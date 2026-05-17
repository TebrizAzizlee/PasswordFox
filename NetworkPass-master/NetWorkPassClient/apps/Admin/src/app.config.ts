import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { appRoutes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { csrfInterceptor } from './AuthServices/data-access/csrf.interceptor';
import { authInterceptor } from './AuthServices/data-access/auth.interceptor';
import { refreshInterceptor } from './AuthServices/data-access/refresh.interceptor';
import { AuthService } from './AuthServices/data-access/auth.service';
import { firstValueFrom } from 'rxjs';


export const appConfig: ApplicationConfig = {
  providers: [provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
   provideRouter(appRoutes),
   provideHttpClient(withInterceptors([authInterceptor,csrfInterceptor,refreshInterceptor])),provideAppInitializer(()=>{
    const authService=inject(AuthService);
     return firstValueFrom(
        authService.initialize()
      );
   } )
  ],
};
