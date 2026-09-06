import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../shared/constants/api-routes/content-management-api.routes';
import { ReactionType } from '../../../shared/contracts/reaction-type';
import { Result } from '../../../shared/contracts/result';
import { Paged } from '../../../shared/contracts/paged';
import { PostReactionSummaryDto } from '../contracts/post-reaction-summary.dto';

@Injectable({
    providedIn: 'root'
})
export class PostReactionService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    getPostReactions(postId: string, currentPage: number = 1, pageSize: number = 20): Observable<Result<Paged<PostReactionSummaryDto>>> {
        const params = new HttpParams()
            .set('currentPage', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<PostReactionSummaryDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.PostReactions(postId)}`, { params });
    }

    addOrChangePostReaction(postId: string, type: ReactionType): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.PostReactions(postId)}`, { type });
    }

    removePostReaction(postId: string): Observable<Result> {
        return this.http.delete<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.PostReactions(postId)}`);
    }
}
