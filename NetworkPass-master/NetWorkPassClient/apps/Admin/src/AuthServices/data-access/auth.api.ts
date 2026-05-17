import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginRequest } from './AuthModels/login-request';
import { LoginResponse } from './AuthModels/login-response';
import { environment } from '../../environments/environment';



@Injectable({
  providedIn: 'root'
})
export class AuthApi {

  private readonly http = inject(HttpClient);

  login(request: LoginRequest): Observable<LoginResponse> {

    return this.http.post<LoginResponse>(
      `${environment.apiBaseUrl}/auth/login`,
      request,
      {
        withCredentials: true
      }
    );
  }

  refresh(): Observable<LoginResponse> {

    return this.http.post<LoginResponse>(
      `${environment.apiBaseUrl}/auth/refresh-token`,
      {},
      {
        withCredentials: true
      }
    );
  }

  me() {

    return this.http.get<any>(
      `${environment.apiBaseUrl}/auth/me`,
      {
        withCredentials: true
      }
    );
  }

  logout() {

    return this.http.post(
      `${environment.apiBaseUrl}/auth/logout`,
      {},
      {
        withCredentials: true
      }
    );
  }
}
