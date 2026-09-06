import { ProfileSummaryDto } from '../../profiles/profiles/contracts/profile-summary.dto';
import { ProjectOverviewDto } from '../../projects/contracts/project-summary.dto';
import { CommunitySummaryDto } from '../../content-management/communities/communities/contracts/community-summary.dto';
import { JobSummaryDto } from '../../jobs/jobs/contracts/job-summary.dto';
import { ProblemSummaryDto } from '../../discussions/problems/problems/contracts/problem-summary.dto';
import { PostOverviewDto } from '../../content-management/posts/contracts/post-model.dto';
import { ProjectStatus } from '../../projects/enums/project-status.enum';
import { CommunityType } from '../../shared/contracts/community-type';
import { JobType } from '../../jobs/enums/job-type.enum';
import { SalaryType } from '../../jobs/enums/salary-type.enum';
import { DifficultyLevel } from '../../discussions/shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../discussions/problems/enums/problem-status.enum';

export interface SearchHit<T> {
    document: T;
    score: number;
}

export interface SearchResult<T> {
    hits: SearchHit<T>[];
    total: number;
}

export interface GlobalSearchResultDto {
    profiles: ProfileSummaryDto[];
    projects: ProjectOverviewDto[];
    communities: CommunitySummaryDto[];
    jobs: JobSummaryDto[];
    problems: ProblemSummaryDto[];
    posts: PostOverviewDto[];
}

export interface BaseSearchQuery {
    searchTerm: string;
    currentPage?: number;
    page?: number;
    pageSize?: number;
}

export interface GlobalSearchQuery {
    searchTerm: string;
    topResultsPerCategory?: number;
}

export interface PostsSearchQuery extends BaseSearchQuery {
    minCreatedAt?: string | null;
    maxCreatedAt?: string | null;
    tags?: string[] | null;
    topics?: string[] | null;
}

export interface ProfilesSearchQuery extends BaseSearchQuery {
    requiredSkills?: string[] | null;
    currentProfileId?: string | null;
}

export interface ProjectsSearchQuery extends BaseSearchQuery {
    status?: ProjectStatus | null;
    minCreatedAt?: string | null;
    maxCreatedAt?: string | null;
    requiredSkills?: string[] | null;
    minContributors?: number | null;
    maxContributors?: number | null;
    minRate?: number | null;
}

export interface CommunitiesSearchQuery extends BaseSearchQuery {
    type?: CommunityType | null;
}

export interface JobsSearchQuery extends BaseSearchQuery {
    type?: JobType | null;
    salaryType?: SalaryType | null;
    minSalary?: number | null;
    maxSalary?: number | null;
    minCreatedAt?: string | null;
    maxCreatedAt?: string | null;
}

export interface ProblemsSearchQuery extends BaseSearchQuery {
    minCreatedAt?: string | null;
    maxCreatedAt?: string | null;
    level?: DifficultyLevel | null;
    status?: ProblemStatus | null;
}

export type SearchCategory =
    | 'People'
    | 'Posts'
    | 'Projects'
    | 'Communities'
    | 'Jobs'
    | 'Problems';
