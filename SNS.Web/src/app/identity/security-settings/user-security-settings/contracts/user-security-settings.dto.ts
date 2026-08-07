import { MfaProvider } from "../../../shared/contracts/mfa-provider.enum";
import { CommunicationMethod } from "../../../shared/contracts/communication-method.enum";

export interface UserSecuritySettingsDto {
    isMfaEnabled: boolean;
    isAuthenticatorLinked: boolean;
    mfaProvider: MfaProvider | null;
    recoveryEmail: string | null;
    defaultCommunicationMethod: CommunicationMethod;
    activeRecoveryCodesCount: number;
}
