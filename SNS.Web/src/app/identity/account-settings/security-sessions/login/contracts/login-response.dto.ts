import { StatusCode } from "../../../../../shared/contracts/status-code";
import { MfaProvider } from "../../../../shared/contracts/mfa-provider.enum";

export interface LoginResponse {
    userId?: string;
    deviceId?: string;
    accessToken?: string;
    refreshToken?: string;
    challengeToken?: string;
    suspendedUntil?: Date;
    suspensionReason?: string;
    requiresTwoFactor: boolean;
    isMfaRequired?: boolean;
    suspensionReasonCode?: StatusCode;
    mfaProviderType: MfaProvider;
}
