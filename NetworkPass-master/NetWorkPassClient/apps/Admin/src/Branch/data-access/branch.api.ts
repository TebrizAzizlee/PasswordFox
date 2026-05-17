import { HttpClient } from '@angular/common/http';

import { inject, Injectable } from '@angular/core';
import { BranchDto } from './models/BranchDto';
import { CreateBranchRequest } from './models/CreateBranchRequest';
import { PagedResponse } from './models/PagedResponse';

@Injectable({ providedIn: 'root' })
export class BranchApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'https://localhost:7036/branches';
  getAll(page=1, pageSize=10) {
    return this.http.get<PagedResponse<BranchDto>>(this.baseUrl,
      {
        params:{
          page,pageSize
        }
      }
    );
  }
  create(request: CreateBranchRequest) {
    return this.http.post(this.baseUrl, request);
  }
}
