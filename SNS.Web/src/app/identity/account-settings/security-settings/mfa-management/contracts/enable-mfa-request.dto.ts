import { MfaProvider } from "../../../../shared/contracts/mfa-provider.enum";

export interface EnableMFACommand {
    mfaProvider: MfaProvider;
}
