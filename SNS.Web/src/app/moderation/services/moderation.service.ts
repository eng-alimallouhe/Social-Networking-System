import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { IDENTITY_API_ROUTES } from '../../shared/constants/api-routes/identity-api.routes';
import { Result } from '../../shared/contracts/result';
import {
    ReportPostRequest,
    ReportCommentRequest,
    ReportUserProfileRequest,
    ReportRatingRequest,
    ReportProjectRequest,
    ReportCompanyRequest,
    ReportJobRequest
} from '../contracts';

@Injectable({
    providedIn: 'root'
})
export class ModerationService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}${IDENTITY_API_ROUTES.Moderation}`;

    reportPost(postId: string, request: ReportPostRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(`${this.apiUrl}/posts/${postId}/report`, request);
    }

    reportComment(commentId: string, request: ReportCommentRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(`${this.apiUrl}/comments/${commentId}/report`, request);
    }

    reportUserProfile(userProfileId: string, request: ReportUserProfileRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(`${this.apiUrl}/user-profiles/${userProfileId}/report`, request);
    }

    reportRating(ratingId: string, request: ReportRatingRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(`${this.apiUrl}/ratings/${ratingId}/report`, request);
    }

    reportProject(projectId: string, request: ReportProjectRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(`${this.apiUrl}/projects/${projectId}/report`, request);
    }

    reportCompany(companyId: string, request: ReportCompanyRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(`${this.apiUrl}/companies/${companyId}/report`, request);
    }

    reportJob(jobId: string, request: ReportJobRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(`${this.apiUrl}/jobs/${jobId}/report`, request);
    }
}