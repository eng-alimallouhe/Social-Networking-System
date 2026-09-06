import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { SEARCH_API_ROUTES } from '../../shared/constants/api-routes/search-api.routes';
import { Result } from '../../shared/contracts/result';
import {
    BaseSearchQuery,
    GlobalSearchResultDto,
    SearchResult,
    PostsSearchQuery,
    ProfilesSearchQuery,
    ProjectsSearchQuery,
    CommunitiesSearchQuery,
    JobsSearchQuery,
    ProblemsSearchQuery
} from '../contracts/search.dto';
import { PostOverviewDto } from '../../content-management/posts/contracts/post-model.dto';
import { ProfileSummaryDto } from '../../profiles/profiles/contracts/profile-summary.dto';
import { ProjectOverviewDto } from '../../projects/contracts/project-summary.dto';
import { CommunitySummaryDto } from '../../content-management/communities/communities/contracts/community-summary.dto';
import { JobSummaryDto } from '../../jobs/jobs/contracts/job-summary.dto';
import { ProblemSummaryDto } from '../../discussions/problems/problems/contracts/problem-summary.dto';

@Injectable({
    providedIn: 'root'
})
export class SearchService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    globalSearch(searchTerm: string, topResultsPerCategory?: number): Observable<Result<GlobalSearchResultDto>> {
        let params = new HttpParams().set('searchTerm', searchTerm ?? '');
        if (topResultsPerCategory !== undefined && topResultsPerCategory !== null) {
            params = params.set('topResultsPerCategory', topResultsPerCategory.toString());
        }
        return this.http.get<Result<GlobalSearchResultDto>>(`${this.baseUrl}${SEARCH_API_ROUTES.GlobalSearch}`, { params });
    }

    searchPosts(query: PostsSearchQuery): Observable<Result<SearchResult<PostOverviewDto>>> {
        let params = this.buildBaseParams(query);
        if (query.minCreatedAt) params = params.set('minCreatedAt', query.minCreatedAt);
        if (query.maxCreatedAt) params = params.set('maxCreatedAt', query.maxCreatedAt);
        if (query.tags) {
            query.tags.forEach(tag => {
                params = params.append('tags', tag);
            });
        }
        if (query.topics) {
            query.topics.forEach(topic => {
                params = params.append('topics', topic);
            });
        }
        return this.http.get<Result<SearchResult<PostOverviewDto>>>(`${this.baseUrl}${SEARCH_API_ROUTES.Posts}`, { params });
    }

    searchProfiles(query: ProfilesSearchQuery): Observable<Result<SearchResult<ProfileSummaryDto>>> {
        let params = this.buildBaseParams(query);
        if (query.requiredSkills) {
            query.requiredSkills.forEach(skill => {
                params = params.append('requiredSkills', skill);
            });
        }
        if (query.currentProfileId) {
            params = params.set('currentProfileId', query.currentProfileId);
        }
        return this.http.get<Result<SearchResult<ProfileSummaryDto>>>(`${this.baseUrl}${SEARCH_API_ROUTES.Profiles}`, { params });
    }

    searchProjects(query: ProjectsSearchQuery): Observable<Result<SearchResult<ProjectOverviewDto>>> {
        let params = this.buildBaseParams(query);
        if (query.status !== undefined && query.status !== null) params = params.set('status', query.status.toString());
        if (query.minCreatedAt) params = params.set('minCreatedAt', query.minCreatedAt);
        if (query.maxCreatedAt) params = params.set('maxCreatedAt', query.maxCreatedAt);
        if (query.requiredSkills) {
            query.requiredSkills.forEach(skill => {
                params = params.append('requiredSkills', skill);
            });
        }
        if (query.minContributors !== undefined && query.minContributors !== null) {
            params = params.set('minContributors', query.minContributors.toString());
        }
        if (query.maxContributors !== undefined && query.maxContributors !== null) {
            params = params.set('maxContributors', query.maxContributors.toString());
        }
        if (query.minRate !== undefined && query.minRate !== null) {
            params = params.set('minRate', query.minRate.toString());
        }
        return this.http.get<Result<SearchResult<ProjectOverviewDto>>>(`${this.baseUrl}${SEARCH_API_ROUTES.Projects}`, { params });
    }

    searchCommunities(query: CommunitiesSearchQuery): Observable<Result<SearchResult<CommunitySummaryDto>>> {
        let params = this.buildBaseParams(query);
        if (query.type !== undefined && query.type !== null) params = params.set('type', query.type.toString());
        return this.http.get<Result<SearchResult<CommunitySummaryDto>>>(`${this.baseUrl}${SEARCH_API_ROUTES.Communities}`, { params });
    }

    searchJobs(query: JobsSearchQuery): Observable<Result<SearchResult<JobSummaryDto>>> {
        let params = this.buildBaseParams(query);
        if (query.type !== undefined && query.type !== null) params = params.set('type', query.type.toString());
        if (query.salaryType !== undefined && query.salaryType !== null) params = params.set('salaryType', query.salaryType.toString());
        if (query.minSalary !== undefined && query.minSalary !== null) params = params.set('minSalary', query.minSalary.toString());
        if (query.maxSalary !== undefined && query.maxSalary !== null) params = params.set('maxSalary', query.maxSalary.toString());
        if (query.minCreatedAt) params = params.set('minCreatedAt', query.minCreatedAt);
        if (query.maxCreatedAt) params = params.set('maxCreatedAt', query.maxCreatedAt);
        return this.http.get<Result<SearchResult<JobSummaryDto>>>(`${this.baseUrl}${SEARCH_API_ROUTES.Jobs}`, { params });
    }

    searchProblems(query: ProblemsSearchQuery): Observable<Result<SearchResult<ProblemSummaryDto>>> {
        let params = this.buildBaseParams(query);
        if (query.minCreatedAt) params = params.set('minCreatedAt', query.minCreatedAt);
        if (query.maxCreatedAt) params = params.set('maxCreatedAt', query.maxCreatedAt);
        if (query.level !== undefined && query.level !== null) params = params.set('level', query.level.toString());
        if (query.status !== undefined && query.status !== null) params = params.set('status', query.status.toString());
        return this.http.get<Result<SearchResult<ProblemSummaryDto>>>(`${this.baseUrl}${SEARCH_API_ROUTES.Problems}`, { params });
    }

    private buildBaseParams(query: BaseSearchQuery): HttpParams {
        let params = new HttpParams().set('searchTerm', query.searchTerm !== undefined && query.searchTerm !== null ? query.searchTerm : '');
        const page = query.currentPage ?? query.page;
        if (page !== undefined && page !== null) {
            params = params.set('page', page.toString());
        }
        if (query.pageSize !== undefined && query.pageSize !== null) {
            params = params.set('pageSize', query.pageSize.toString());
        }
        return params;
    }
}
