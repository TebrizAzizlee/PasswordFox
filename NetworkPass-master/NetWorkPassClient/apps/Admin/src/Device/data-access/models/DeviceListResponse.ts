
import { DeviceDto } from "./DeviceDto";



export interface DeviceListResponse {

  items: DeviceDto[];

  totalCount: number;

  page: number;

  pageSize: number;

  totalPages: number;

  hasNext: boolean;

  hasPrevious: boolean;

}

