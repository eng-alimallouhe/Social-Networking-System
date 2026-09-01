import { ResumeSkillLevel } from '../../enums/resume-skill-level.enum';

export interface UpdateResumeSkillCommand {
    resumeId?: string;
    skillId?: string;
    skillName: string;
    level: ResumeSkillLevel;
}
