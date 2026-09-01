import { ResumeSkillLevel } from '../../enums/resume-skill-level.enum';

export interface AddResumeSkillCommand {
    resumeId?: string;
    skillName: string;
    level: ResumeSkillLevel;
}
