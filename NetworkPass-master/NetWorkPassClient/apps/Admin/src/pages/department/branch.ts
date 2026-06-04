import { ChangeDetectionStrategy, Component, computed, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { Breadcrumbservice } from '../../services/breadcrumbservice';
import Blank from '../../components/blank/blank';

import { BranchService } from '../../Branch/data-access/branch.service';
import { BranchStore } from '../../Branch/data-access/branch.store';
import { DatePipe } from '@angular/common';
import { RouterLink } from "@angular/router";


@Component({
  imports: [Blank, DatePipe, RouterLink],
  templateUrl: './branch.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export  class Branch implements OnInit {
readonly #breadcrumb=inject(Breadcrumbservice);

private readonly service = inject(BranchService);
  readonly branches = inject(BranchStore).branches;
   readonly totalCount = computed(
    () => this.branches().length
  );
readonly onlineBranchCount =
computed( () => this.branches() .filter(x => x.status === 'Online') .length );
 readonly totalAlerts = computed( () => this.branches() .reduce( (sum, x) => sum + x.alertCount, 0 ) );
  readonly averageHealthScore = computed(() => { const items = this.branches(); if (items.length === 0) { return 0; }
  return Math.round( items.reduce( (sum, x) => sum + x.healthScore, 0 ) / items.length ); });
  ngOnInit(): void {
    this.#breadcrumb.setDepartment();
     this.service.loadBranches();
  }
}
