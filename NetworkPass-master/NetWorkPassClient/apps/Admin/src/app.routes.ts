
import { Route } from '@angular/router';

export const appRoutes: Route[] = [

  // 🔓 PUBLIC ROUTES
  {
    path: 'login',

    loadComponent: () =>
      import('./pages/login/login')
        .then(m => m.LoginComponent)
  },

  // 🔒 AUTH ORCHESTRATION LAYOUT
  {
    path: '',

    loadComponent: () =>
      import(
        './pages/layouts/protected-layout/protected-layout'
      ).then(
        m => m.ProtectedLayoutComponent
      ),

    children: [

      // 🔥 MAIN APP LAYOUT
      {
        path: '',

        loadComponent: () =>
          import('./pages/layouts/layouts')
            .then(m => m.default),

        children: [

          // DASHBOARD
          {
            path: '',

            loadComponent: () =>
              import('./pages/dashboard/dashboard')
                .then(m => m.Dashboard)
          },

          // DEPARTMENTS
          {
            path: 'departments',

            loadChildren: () =>
              import('./pages/department/router').then(m=>m.default)

          }
        ]
      }
    ]
  },

  // 🔥 FALLBACK
  {
    path: '**',

    redirectTo: ''
  }
];

