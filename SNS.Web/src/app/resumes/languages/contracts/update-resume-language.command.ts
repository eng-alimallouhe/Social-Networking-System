import { Language } from '../../enums/language.enum';
import { LanguageLevel } from '../../enums/language-level.enum';

export interface UpdateResumeLanguageCommand {
    resumeId?: string;
    languageId?: string;
    language: Language;
    level: LanguageLevel;
}
