import { MfaProvider } from "../../../../shared/contracts/mfa-provider.enum";

export interface ChangeMfaProviderCommand {
    newProvider: MfaProvider;
}
