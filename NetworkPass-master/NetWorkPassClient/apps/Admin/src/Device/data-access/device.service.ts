
import { inject, Injectable } from '@angular/core';

import { DeviceApi } from './device.api';
import { DeviceStore } from './device.store';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {

  private readonly api =
    inject(DeviceApi);

  private readonly store =
    inject(DeviceStore);

  loadDevices(
    page = 1,
    pageSize = 10
  ) {

    this.store.loading.set(true);

    this.api
      .getAll(page, pageSize)
      .subscribe({

        next: response => {

          this.store.devices.set(
            response.items
          );

          this.store.totalCount.set(
            response.totalCount
          );

          this.store.page.set(
            response.page
          );

          this.store.totalPages.set(
            response.totalPages
          );

          this.store.loaded.set(true);
        },

        error: err => {

          console.error(
            'Device load failed',
            err
          );
        },

        complete: () => {

          this.store.loading.set(false);
        }
      });
  }
}

