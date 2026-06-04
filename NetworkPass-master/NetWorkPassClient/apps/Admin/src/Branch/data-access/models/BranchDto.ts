

export interface BranchDto {

id: string;

code: string;

name: string;

city: string;

type: string;

status: string;

totalDeviceCount: number;

onlineDeviceCount: number;

degradedDeviceCount: number;

offlineDeviceCount: number;

alertCount: number;

healthScore: number;

isActive: boolean;

lastSeenAt: string | null;
}

