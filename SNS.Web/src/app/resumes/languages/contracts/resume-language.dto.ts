import { Language } from '../../enums/language.enum';
import { LanguageLevel } from '../../enums/language-level.enum';

export interface ResumeLanguageDto {
    id: string;
    resumeId: string;
    language: Language;
    level: LanguageLevel;
}
