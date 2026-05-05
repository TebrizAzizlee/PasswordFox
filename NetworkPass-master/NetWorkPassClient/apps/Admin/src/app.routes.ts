import { Route } from '@angular/router';
import { authGuard } from './guards/auth-guard';


export const appRoutes: Route[] = [

  // 🔓 PUBLIC
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then(m => m.LoginComponent)
  },

  // 🔒 PROTECTED
  {
    path: '',
    loadComponent: () => import('./pages/layouts/layouts'),
    // canActivate: [authGuard],        // 🔥 əlavə et
    canActivateChild: [authGuard],
    children: [

      {
        path: '',
        loadComponent: () => import('./pages/dashboard/dashboard'),
      },

      {
        path: 'departments',
        loadComponent: () => import('./pages/department/department'),
        
      }

    ],
  },

  // fallback
  {
    path: '**',
    redirectTo: ''
  }
];