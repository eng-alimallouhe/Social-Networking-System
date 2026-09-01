import { Template } from '../../enums/template.enum';
import { SupportedLanguage } from '../../../shared/contracts/supported-language.enum';
import { ResumeEducationDto } from '../../educations/contracts/resume-education.dto';
import { ResumeExperienceDto } from '../../experiences/contracts/resume-experience.dto';
import { ResumeCertificateDto } from '../../certificates/contracts/resume-certificate.dto';
import { ResumeLanguageDto } from '../../languages/contracts/resume-language.dto';
import { ResumeSkillDto } from '../../skills/contracts/resume-skill.dto';
import { ResumeProjectDto } from '../../projects/contracts/resume-project.dto';

export interface ResumeDetailsDto {
    id: string;
    ownerId: string;
    personalPictureUrl?: string | null;
    syncProfilePicture: boolean;
    title: string;
    template: Template;
    summary: string;
    language: SupportedLanguage;
    createdAt: string;
    updatedAt: string;
    educations: ResumeEducationDto[];
    experiences: ResumeExperienceDto[];
    certificates: ResumeCertificateDto[];
    languages: ResumeLanguageDto[];
    skills: ResumeSkillDto[];
    projects: ResumeProjectDto[];
}
