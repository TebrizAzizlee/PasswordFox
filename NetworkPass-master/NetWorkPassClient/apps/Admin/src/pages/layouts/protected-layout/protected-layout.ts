import {

} from '@angular/core';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
} from '@angular/core';

import { Router, RouterOutlet } from '@angular/router';

import { CommonModule } from '@angular/common';
import { AuthStore } from '../../../AuthServices/data-access/auth.store';


@Component({
  selector: 'app-protected-layout',

  standalone: true,

  imports: [CommonModule, RouterOutlet],

  templateUrl: './protected-layout.html',

  styleUrl: './protected-layout.css',

  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProtectedLayoutComponent {
  private readonly authStore = inject(AuthStore);

  private readonly router = inject(Router);

  readonly isLoading = computed(() => {
    const status = this.authStore.status();

    return status === 'unknown' || status === 'refreshing';
  });

  constructor() {
    effect(() => {
      const status = this.authStore.status();

      if (status === 'unauthenticated') {
        queueMicrotask(() => {
          this.router.navigateByUrl('/login');
        });
      }
    });
  }
}
