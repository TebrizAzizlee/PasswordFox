import { Routes } from "@angular/router";
 const router:Routes=[
  {
    path:'',
    loadComponent:()=> import('./device').then(x=>x.Device)
  },
  {
    path:'add',
    loadComponent:()=>import('./create/create').then(x=>x.Create)
  },
    {
    path:'edit/:id',
    loadComponent:()=>import('./create/create').then(x=>x.Create)
  }

]

export default router;
