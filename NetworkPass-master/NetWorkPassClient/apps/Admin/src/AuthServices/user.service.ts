import { inject, Injectable } from '@angular/core';
import { ApiService } from './api-service';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
    private api = inject(ApiService);

  getUsers(): Observable<User> {
    return this.api.get('users');
  }

  getUserById(id: number): Observable<User> {
    return this.api.get(`users/${id}`);
  }
}
