import { inject, Injectable } from '@angular/core';
import { BranchApi } from './branch.api';
import { BranchStore } from './branch.store';
import { CreateBranchRequest } from './models/CreateBranchRequest';

@Injectable({
  providedIn: 'root',
})
export class BranchService {
  private readonly api = inject(BranchApi);

  private readonly store = inject(BranchStore);
  readonly page = inject(BranchStore).page;
readonly totalPages = inject(BranchStore).totalPages;
  loadBranches() {
    this.store.loading.set(true);

    this.api.getAll().subscribe({
      next: (response) => {
        this.store.branches.set(response.items);
         this.store.page.set(response.page);
         this.store.totalPages.set(response.totalPages);
         this.store.totalCount.set(response.totalCount);
        this.store.loaded.set(true);
      },

      error: (error) => {
        console.error('Departments load failed', error);
      },

      complete: () => {
        this.store.loading.set(false);
      },
    });
  }
  createBranch(request: CreateBranchRequest) {
    return this.api.create(request);
  }
}
