import { ProfileSkillDto } from './profile-skill.dto';
import { AcademicRecordSummaryDto } from './academic-record-summary.dto';

export interface ProfileDetailsDto {
    id: string;
    fullName: string;
    bio?: string | null;
    profilePictureUrl?: string | null;
    specialization?: string | null;
    followersCount: number;
    followingsCount: number;
    viewsCount: number;
    skills: ProfileSkillDto[];
    academicRecordSummaryDtos: AcademicRecordSummaryDto[];
    location?: string | null;
    gitHubUrl?: string | null;
    linkedInUrl?: string | null;
    xUrl?: string | null;
    facebookUrl?: string | null;
    website?: string | null;
    isFollowedByViewer: boolean;
    isBlockedByViewer: boolean;
    isViewerOwner: boolean;
    isBlockingViewer: boolean;
}
