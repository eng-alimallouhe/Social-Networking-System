export interface ActiveSessionDto {
    sessionId: string;
    deviceName: string;
    browser: string;
    operatingSystem: string;
    ipAddress: string;
    location: string | null;
    createdAt: Date;
    isCurrentSession: boolean;
}

export interface RegisteredDeviceDto {
    deviceId: string;
    deviceName: string;
    operatingSystem: string;
    firstSeenAt: Date;
    lastSeenAt: Date;
}

export interface UserActiveSessionsAndDevicesResult {
    activeSessions: ActiveSessionDto[];
    registeredDevices: RegisteredDeviceDto[];
}