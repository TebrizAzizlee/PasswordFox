import { computed, Injectable, signal } from "@angular/core";

export type AuthStatus =
  | 'unknown'
  | 'authenticated'
  | 'refreshing'
  | 'unauthenticated';


export interface CurrentUser {
  userId: string;
  email: string;
  userName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthStore {

  accessToken = signal<string | null>(null);

  user = signal<CurrentUser | null>(null);

  status = signal<AuthStatus>('unknown');

  isAuthenticated = computed(() =>
    this.status() === 'authenticated'
  );
}
