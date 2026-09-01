import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../shared/constants/api-routes/content-management-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { ReactionType } from '../../../shared/contracts/reaction-type';
import { CommentReactionSummaryDto } from '../contracts/comment-reaction-summary.dto';

@Injectable({
    providedIn: 'root'
})
export class CommentReactionService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    getCommentReactions(commentId: string, currentPage: number = 1, pageSize: number = 20): Observable<Result<Paged<CommentReactionSummaryDto>>> {
        const params = new HttpParams()
            .set('page', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<CommentReactionSummaryDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommentReactions(commentId)}`, { params });
    }

    addOrChangeReaction(commentId: string, type: ReactionType): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommentReactions(commentId)}`, { type });
    }

    removeReaction(commentId: string): Observable<Result> {
        return this.http.delete<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommentReactions(commentId)}`);
    }
}
