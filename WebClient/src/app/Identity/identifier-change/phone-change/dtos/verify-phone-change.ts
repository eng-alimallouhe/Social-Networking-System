export interface VerifyPhoneChangeDto {
    userId: string;
    token: string;
    code: string;
}