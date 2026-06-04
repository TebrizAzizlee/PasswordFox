
import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { DeviceMonitoringResponse }
from './models/DeviceMonitoringResponse';

@Injectable({
  providedIn: 'root'
})
export class DeviceApi {

  private readonly http =
    inject(HttpClient);

  private readonly apiUrl =
    'https://localhost:7036/devices/monitoring';

  getAll(
    page= 1,
    pageSize = 10,
    search?: string
  ) {

    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (search) {
      params = params.set(
        'search',
        search
      );
    }

    return this.http.get<
      DeviceMonitoringResponse>(
        this.apiUrl,
        { params }
      );
  }
}

