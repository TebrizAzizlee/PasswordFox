import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
   private http = inject(HttpClient);

  get<T>(url: string): Observable<T> {
    return this.http.get<T>(`/pass/${url}`);
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(`/pass/${url}`, body);
  }

  put<T>(url: string, body: any): Observable<T> {
    return this.http.put<T>(`/pass/${url}`, body);
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(`/pass/${url}`);
  }
}
