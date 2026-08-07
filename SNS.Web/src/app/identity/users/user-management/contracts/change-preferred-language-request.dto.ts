import { SupportedLanguage } from '../../../../shared/contracts/supported-language.enum';

export interface ChangePreferredLanguageRequest {
    preferredLanguage: SupportedLanguage;
}
