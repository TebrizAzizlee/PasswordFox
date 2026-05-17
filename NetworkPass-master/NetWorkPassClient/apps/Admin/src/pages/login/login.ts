import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal,
} from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Router } from '@angular/router';

import { finalize } from 'rxjs';

import { AuthService } from '../../AuthServices/data-access/auth.service';

import { AuthStore } from '../../AuthServices/data-access/auth.store';

@Component({
  selector: 'app-login',

  standalone: true,

  imports: [ReactiveFormsModule],

  templateUrl: './login.html',

  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);

  private readonly authService = inject(AuthService);

  private readonly authStore = inject(AuthStore);

  private readonly router = inject(Router);

  readonly loading = signal(false);

  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    loginIdentifier: ['', [Validators.required]],

    password: ['', [Validators.required]],
  });

  readonly isAuthenticated = computed(() => this.authStore.isAuthenticated());

  constructor() {
    // 🔥 artıq login olubsa redirect
    if (this.isAuthenticated()) {
      queueMicrotask(() => {
        this.router.navigateByUrl('/');
      });
    }
  }

  submit() {
    // 🔥 spam protection
    if (this.loading()) {
      return;
    }

    // 🔥 invalid form
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.loading.set(true);

    this.errorMessage.set(null);

    this.authService
      .login(this.form.getRawValue())
      .pipe(
        finalize(() => {
          this.loading.set(false);
        }),
      )
      .subscribe({
        next: () => {
          this.router.navigateByUrl('/');
        },

        error: () => {
          this.errorMessage.set('İstifadəçi adı və ya şifrə yanlışdır.');
        },
      });
  }
}
