export interface SessionDetailsDto {
    sessionId: string;
    userId: string;
    loginAt: Date;
    lastSeenAt: Date;
    logoutAt: Date | null;
    ipAddress: string;
    city: string;
    country: string;
    durationMinutes: number;
    revokedAt: Date | null;
    isRevoked: boolean;
    revokedReason: string | null;
    deviceName: string;
    browser: string;
    operatingSystem: string;
    deviceVendor: string | null;
    isDeviceTrusted: boolean;
    deviceFirstSeenAt: Date;
    isViewrOwner: boolean;
    isViwerCurrentSession: boolean;
}