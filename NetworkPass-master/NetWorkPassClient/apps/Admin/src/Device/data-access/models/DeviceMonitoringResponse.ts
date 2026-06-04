
import { DeviceMonitoringDto } from './DeviceMonitoringDto';

export interface DeviceMonitoringResponse {

  items: DeviceMonitoringDto[];

  totalCount: number;

  page: number;

  pageSize: number;

  totalPages: number;

  hasNext: boolean;

  hasPrevious: boolean;

}

