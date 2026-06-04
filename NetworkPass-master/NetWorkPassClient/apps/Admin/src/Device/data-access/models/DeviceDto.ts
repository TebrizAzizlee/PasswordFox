
export interface DeviceDto {

  id: string;

  name: string;

  ipAddress: string;

  type: string;

  status: string;

  cpuUsage: number | null;

  memoryUsage: number | null;

  pingLatency: number | null;

  lastSeenAt: string | null;

}
