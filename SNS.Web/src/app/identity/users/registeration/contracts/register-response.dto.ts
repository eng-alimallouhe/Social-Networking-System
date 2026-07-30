export interface RegisterResponse {
    userId?: string;
    challengeToken?: string;
    requiresVerification: boolean;
}
