export interface DeviceRealtimeDto {

  id: string;

  branchId: string;

  name: string;

  status: number;

  pingLatency: number | null;

  cpuUsage: number | null;

  memoryUsage: number | null;

  temperature: number | null;

  lastSeenAt: string | null;
}
