import { CanActivateFn, Router } from '@angular/router';

import { inject } from '@angular/core';

import { AuthStore } from './auth.store';

export const authGuard: CanActivateFn = () => {
  const authStore = inject(AuthStore);

  const router = inject(Router);

  const status = authStore.status();

  // 🔥 initialize davam edir
  if (status === 'unknown' || status === 'refreshing') {
    return false;
  }

  // 🔥 authenticated
  if (status === 'authenticated') {
    return true;
  }

  // 🔥 unauthenticated
  router.navigateByUrl('/login');

  return false;
};
