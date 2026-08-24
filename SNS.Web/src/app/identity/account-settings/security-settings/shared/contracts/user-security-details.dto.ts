import { CommunicationMethod } from '../../../../shared/contracts/communication-method.enum';

export interface UserSecurityDetailsDto {
    isMfaEnabled: boolean;
    mfaProvider: string;
    isAuthenticatorAppLinked: boolean;
    passkeysCount: number;
    lastPasswordChange: string;
    totalDevicesCount: number;
    recoveryEmail: string | null;
    usedRecoveryCodesCount: number;
    unusedRecoveryCodesCount: number;
    defualtCommunicationMethod: CommunicationMethod;
}