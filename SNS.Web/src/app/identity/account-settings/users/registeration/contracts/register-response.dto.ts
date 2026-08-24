export interface RegisterResponse {
    userId: string;
    token?: string;
    requiresVerification: boolean;
}
