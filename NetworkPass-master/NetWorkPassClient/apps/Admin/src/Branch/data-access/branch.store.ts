import { Injectable, signal } from '@angular/core';
import { BranchDto } from './models/BranchDto';


@Injectable({
  providedIn: 'root',
})
export class BranchStore {
  branches = signal<BranchDto[]>([]);
page = signal(1);

totalPages = signal(1);

totalCount = signal(0);
  loading = signal(false);

  loaded = signal(false);
}
