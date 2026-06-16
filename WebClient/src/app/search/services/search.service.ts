import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { GlobalSearchResultDto } from "../dtos/global-search-result.dto";
import { Observable } from "rxjs";
import { ProfileSummaryDto } from "../../social-graph/dtos/profile-summary.dto";
import { PostSummaryDto } from "../../content/posts/dtos/post-summary.dto";
import { ProjectSummaryDto } from "../../projects/dtos/project-summary.dto";
import { CommunitySummaryDto } from "../../communities/dtos/community-summary.dto";
import { JobSummaryDto } from "../../jobs/dtos/job-summary.dto";
import { ProblemSummaryDto } from "../../qa/dtos/problem-summary-dto";
import { SearchResultDto } from "../shared/dtos/search-result.dto";

@Injectable({
    providedIn: 'root'
})
export class SearchService {
    private apiUrl = environment.apiUrl + "search/";
    private http = inject(HttpClient)


    public searchForAll(query: string): Observable<SearchResultDto<GlobalSearchResultDto>> {
        return this.http.get<SearchResultDto<GlobalSearchResultDto>>(`${this.apiUrl}all?query=${query}`);
    }

    public searchForProfiles(query: string): Observable<SearchResultDto<ProfileSummaryDto>> {
        return this.http.get<SearchResultDto<ProfileSummaryDto>>(`${this.apiUrl}profiles?query=${query}`);
    }

    public searchForPosts(query: string): Observable<SearchResultDto<PostSummaryDto>> {
        return this.http.get<SearchResultDto<PostSummaryDto>>(`${this.apiUrl}posts?query=${query}`);
    }

    public searchForProjects(query: string): Observable<SearchResultDto<ProjectSummaryDto>> {
        return this.http.get<SearchResultDto<ProjectSummaryDto>>(`${this.apiUrl}projects?query=${query}`);
    }

    public searchForCommunities(query: string): Observable<SearchResultDto<CommunitySummaryDto>> {
        return this.http.get<SearchResultDto<CommunitySummaryDto>>(`${this.apiUrl}communities?query=${query}`);
    }

    public searchForJobs(query: string): Observable<SearchResultDto<JobSummaryDto>> {
        return this.http.get<SearchResultDto<JobSummaryDto>>(`${this.apiUrl}jobs?query=${query}`);
    }

    public searchForQnA(query: string): Observable<SearchResultDto<ProblemSummaryDto>> {
        return this.http.get<SearchResultDto<ProblemSummaryDto>>(`${this.apiUrl}questions?query=${query}`);
    }
}