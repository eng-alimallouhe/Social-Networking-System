export interface VerifyRecoveryEmailChangeCommand {
    userId: string;
    token: string;
    code: string;
}
