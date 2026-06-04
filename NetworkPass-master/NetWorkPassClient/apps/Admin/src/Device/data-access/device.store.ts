
import { Injectable, signal } from '@angular/core';
import { DeviceMonitoringDto } from './models/DeviceMonitoringDto';

@Injectable({
  providedIn: 'root'
})
export class DeviceStore {

  devices =
    signal<DeviceMonitoringDto[]>([]);

  loading =
    signal(false);

  loaded =
    signal(false);

  totalCount =
    signal(0);

  page =
    signal(1);

  totalPages =
    signal(1);
}



