import { ChangeDetectionStrategy, Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { Breadcrumbservice } from '../../services/breadcrumbservice';
import Blank from '../../components/blank/blank';
import { RouterLink } from '@angular/router';
import { BranchService } from '../../Branch/data-access/branch.service';
import { BranchStore } from '../../Branch/data-access/branch.store';

@Component({
  imports: [Blank,RouterLink],
  templateUrl: './branch.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export  class Branch implements OnInit {
readonly #breadcrumb=inject(Breadcrumbservice);

private readonly service = inject(BranchService);
  readonly branches = inject(BranchStore).branches;

  ngOnInit(): void {
    this.#breadcrumb.setDepartment();
     this.service.loadBranches();
  }
}
