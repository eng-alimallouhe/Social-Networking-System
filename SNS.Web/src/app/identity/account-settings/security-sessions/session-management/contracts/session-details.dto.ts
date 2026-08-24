export interface SessionDetailsDto {
    sessionId: string;
    userId: string;
    loginAt: string;
    lastSeenAt: string;
    logoutAt: string | null;
    ipAddress: string;
    city: string;
    country: string;
    durationMinutes: number;
    revokedAt: string | null;
    isRevoked: boolean;
    revokedReason: string | null;
    deviceName: string;
    browser: string;
    operatingSystem: string;
    deviceVendor: string | null;
    isDeviceTrusted: boolean;
    deviceFirstSeenAt: string;
    isViewrOwner: boolean;
    isViwerCurrentSession: boolean;
}
