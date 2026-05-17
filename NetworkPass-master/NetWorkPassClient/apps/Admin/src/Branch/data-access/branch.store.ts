import { Injectable, signal } from '@angular/core';
import { BranchDto } from './models/BranchDto';


@Injectable({
  providedIn: 'root',
})
export class BranchStore {
  branches = signal<BranchDto[]>([]);

  loading = signal(false);

  loaded = signal(false);
}
