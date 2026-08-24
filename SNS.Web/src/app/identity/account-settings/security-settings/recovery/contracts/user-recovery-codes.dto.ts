export interface UserRecoveryCodesDto {
    usedCodesCount: number;
    unusedCodesCount: number;
    recoveryCodesUsingHistory: RecoveryCodeUsingSnapshot[];
}

export interface RecoveryCodeUsingSnapshot {
    codeId: string;
    usedAt: string | null;
    generatingDate: string;
}