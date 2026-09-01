import { ResumeSkillLevel } from '../../enums/resume-skill-level.enum';

export interface ResumeSkillDto {
    id: string;
    resumeId: string;
    skillName: string;
    level: ResumeSkillLevel;
}
