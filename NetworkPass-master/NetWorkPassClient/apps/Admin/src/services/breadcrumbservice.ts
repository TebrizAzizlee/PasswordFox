import {  } from './breadcrumbservice';
import { Injectable, signal } from '@angular/core';

export interface BreadcrumbModel{
  title:string;
  url:string;
  icon:string;
  isActive?:boolean;
}
@Injectable({
  providedIn: 'root',
})
export class Breadcrumbservice {
readonly breadcrumbdata=signal<BreadcrumbModel[]>([]);

addHome(){
  const dashboard:BreadcrumbModel={
    title:"Dashboard",
    url:'/',
    icon:'bi-speedometer2'
  }
  this.breadcrumbdata.set([{...dashboard}])

}
setDashboard() {
    this.breadcrumbdata.set([
      {
        title: 'Dashboard',
        url: '/dashboard',
        icon: 'bi-speedometer2',
        isActive: true
      }
    ]);
  }
 setDepartment() {
    this.breadcrumbdata.set([

      {
        title: 'Branches',
        url: '/department',
        icon: 'bi-diagram-3',
        isActive: true
      }
    ]);
  }



 setCustom(breadcrumbs: BreadcrumbModel[]) {
    this.breadcrumbdata.set(breadcrumbs);
  }
    clear() {
    this.breadcrumbdata.set([]);
  }
}
