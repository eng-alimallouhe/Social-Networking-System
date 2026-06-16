export interface VerifyEmailChangeDto {
    userId: string;
    token: string;
    code: string;
}