export interface ActiveSessionDto {
    sessionId: string;
    deviceName: string;
    browser: string;
    operatingSystem: string;
    ipAddress: string;
    location: string | null;
    createdAt: string;
    isCurrentSession: boolean;
}
