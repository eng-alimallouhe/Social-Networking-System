import { Language } from '../../enums/language.enum';
import { LanguageLevel } from '../../enums/language-level.enum';

export interface AddResumeLanguageCommand {
    resumeId?: string;
    language: Language;
    level: LanguageLevel;
}
