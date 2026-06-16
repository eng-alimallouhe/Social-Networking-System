import { SkillSummaryDto } from "../../prefrences/dtos/skill-summary.dto";

export interface ProfileSummaryDto {
    id: string;
    fullName: string;
    specialization: string;
    profilePictureUrl: string;
    followersCount: number;
    followingCount: number;
    topThreeSkills: SkillSummaryDto[];
    skillsCount: number;
    reputation: number;
}