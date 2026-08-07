export interface VerifyEmailChangeRequest {
    userId: string;
    token: string;
    code: string;
}
