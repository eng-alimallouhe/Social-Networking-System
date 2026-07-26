export interface ValidateTwoFactorRequest {
    userId: string;
    code: string;
    token: string;
}