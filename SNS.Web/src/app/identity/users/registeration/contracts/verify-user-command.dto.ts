export interface VerifyUserCommand {
    userId: string;
    challengeToken: string;
    code: string;
}
