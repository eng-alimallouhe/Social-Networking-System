import { Template } from '../../enums/template.enum';
import { SupportedLanguage } from '../../../shared/contracts/supported-language.enum';

export interface UpdateResumeCommand {
    resumeId?: string;
    personalPictureUrl?: string | null;
    syncProfilePicture: boolean;
    title: string;
    template: Template;
    summary: string;
    language: SupportedLanguage;
}
