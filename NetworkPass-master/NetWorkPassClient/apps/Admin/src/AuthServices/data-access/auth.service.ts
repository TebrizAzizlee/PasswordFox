
import { Injectable, inject } from '@angular/core';

import {
  catchError,
  EMPTY,
  map,
  of,
  switchMap,
  tap
} from 'rxjs';

import { AuthApi } from './auth.api';
import { AuthStore } from './auth.store';

import { LoginRequest } from './AuthModels/login-request';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly api =
    inject(AuthApi);

  private readonly store =
    inject(AuthStore);

  // LOGIN
  login(request: LoginRequest) {

    return this.api.login(request).pipe(

      map(response => {

        if (!response.accessToken) {
          throw new Error(
            'Access token missing'
          );
        }

        return response.accessToken;
      }),

      tap(accessToken => {

        this.store.accessToken.set(
          accessToken
        );
      }),

      switchMap(() =>
        this.loadCurrentUser()
      ),

      tap(() => {

        this.store.status.set(
          'authenticated'
        );
      })
    );
  }

  // APP STARTUP
  initialize() {

    this.store.status.set(
      'refreshing'
    );

    return this.refresh().pipe(

      switchMap(() =>
        this.loadCurrentUser()
      ),

      tap(() => {

        this.store.status.set(
          'authenticated'
        );
      }),

      catchError(() => {

        this.logoutLocal();

        return of(null);
      })
    );
  }

  // REFRESH TOKEN
  refresh() {

    return this.api.refresh().pipe(

      map(response => {

        if (!response.accessToken) {
          throw new Error(
            'Access token missing'
          );
        }

        return response.accessToken;
      }),

      tap(accessToken => {

        this.store.accessToken.set(
          accessToken
        );
      })
    );
  }

  // CURRENT USER
  loadCurrentUser() {

    return this.api.me().pipe(

      tap(user => {

        this.store.user.set(user);
      })
    );
  }

  // LOCAL LOGOUT
  logoutLocal() {

    this.store.accessToken.set(null);

    this.store.user.set(null);

    this.store.status.set(
      'unauthenticated'
    );
  }

  logout() {

  return this.api.logout().pipe(

    catchError(() => EMPTY),

    tap(() => {
      this.logoutLocal();
    })
  );
}
}

