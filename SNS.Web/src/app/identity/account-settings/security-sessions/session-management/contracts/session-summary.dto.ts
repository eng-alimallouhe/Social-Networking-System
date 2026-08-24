export interface SessionSummaryDto {
    userId: string;
    id: string;
    deviceName: string;
    loginAt: string;
    lastSeenAt: string;
    logoutAt?: string;
    counrty: string;
    city: string;
    durationMinutes: number;
    isRevoked: boolean;
    revokedReason?: string;
}
