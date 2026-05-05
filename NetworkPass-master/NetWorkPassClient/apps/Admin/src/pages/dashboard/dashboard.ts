import { ChangeDetectionStrategy, Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { Breadcrumbservice } from '../../services/breadcrumbservice';
import Blank from '../../components/blank/blank';

@Component({
  imports: [Blank],
  templateUrl: './dashboard.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Dashboard implements OnInit {
readonly #breadcrumb=inject(Breadcrumbservice);

  ngOnInit(): void {
     this.#breadcrumb.setDashboard();
  }
}
