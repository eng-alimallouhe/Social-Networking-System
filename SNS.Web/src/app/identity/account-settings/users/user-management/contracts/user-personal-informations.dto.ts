import { SupportedLanguage } from "../../../../../shared/contracts/supported-language.enum";

export interface PersonalInformationDto {
    userName: string;
    roleName: string;
    email: string;
    preferredLanguage: SupportedLanguage;
    lastPasswordChange: Date;
    location: string;
    lastActiveLocation: string;
    hasActiveDataDownloadRequest: boolean;
}