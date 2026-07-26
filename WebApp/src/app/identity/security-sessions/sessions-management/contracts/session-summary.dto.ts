export interface SessionSummaryDto {
    userId: string;
    id: string;
    deviceName: string;
    loginAt: Date;
    lastSeenAt: Date;
    logoutAt: Date | null;
    counrty: string;
    city: string;
    durationMinutes: number;
    isRevoked: boolean;
    revokedReason: string | null;
}