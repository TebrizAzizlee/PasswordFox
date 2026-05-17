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

  loadBranches() {
    this.store.loading.set(true);

    this.api.getAll().subscribe({
      next: (response) => {
        this.store.branches.set(response.items);

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
