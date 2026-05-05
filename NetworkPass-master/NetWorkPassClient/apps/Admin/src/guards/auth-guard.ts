import { CanActivateChildFn, Router } from '@angular/router';
import { AuthService } from '../AuthServices/authservice';
import { inject } from '@angular/core';
import { filter, map, take } from 'rxjs';

export const authGuard: CanActivateChildFn = (childRoute, state) => {
    const auth = inject(AuthService);
  const router = inject(Router);

 return auth.isInitialized$.pipe(filter(init=>init===true),
take(1),
map(()=>{
  if(!auth.user()){
    router.navigate(['/login']);
    return false;
  }
  return true;
})
)
};

