import { CommunitySummaryDto } from "../../communities/dtos/community-summary.dto";
import { JobSummaryDto } from "../../jobs/dtos/job-summary.dto";
import { PostSummaryDto } from "../../content/posts/dtos/post-summary.dto";
import { ProjectSummaryDto } from "../../projects/dtos/project-summary.dto";
import { ProfileSummaryDto } from "../../social-graph/dtos/profile-summary.dto";
import { ProblemSummaryDto } from "../../qa/dtos/problem-summary-dto";

export interface GlobalSearchResultDto {
    profiles: ProfileSummaryDto[];
    jobs: JobSummaryDto[];
    communities: CommunitySummaryDto[];
    posts: PostSummaryDto[];
    projects: ProjectSummaryDto[];
    questions: ProblemSummaryDto[];
}