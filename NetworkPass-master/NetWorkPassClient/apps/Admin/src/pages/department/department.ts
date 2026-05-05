import { ChangeDetectionStrategy, Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { Breadcrumbservice } from '../../services/breadcrumbservice';
import Blank from '../../components/blank/blank';

@Component({
  imports: [Blank],
  templateUrl: './department.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Department implements OnInit {
readonly #breadcrumb=inject(Breadcrumbservice);

  ngOnInit(): void {
    this.#breadcrumb.setDepartment();
  }
}
