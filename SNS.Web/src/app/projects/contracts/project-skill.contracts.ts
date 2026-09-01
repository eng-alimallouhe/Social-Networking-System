export interface AddProjectSkillCommand {
    projectId: string;
    skillId: string;
    isPrimary: boolean;
    level: number;
}
