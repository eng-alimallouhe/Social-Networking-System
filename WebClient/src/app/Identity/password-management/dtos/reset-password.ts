export interface ResetPasswordDto {
    userId: string;
    code: string;
    newPassword: string;
}