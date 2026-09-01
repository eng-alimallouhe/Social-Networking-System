import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../shared/constants/api-routes/content-management-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { CommentSummaryDto } from '../contracts/comment-summary.dto';
import { CommentDetailsDto } from '../contracts/comment-details.dto';
import { CreateCommentCommand } from '../contracts/create-comment.command';
import { UpdateCommentCommand } from '../contracts/update-comment.command';

@Injectable({
    providedIn: 'root'
})
export class CommentsService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    createComment(command: CreateCommentCommand): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.Comments}`, command);
    }

    updateComment(commentId: string, command: UpdateCommentCommand): Observable<Result> {
        return this.http.put<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommentById(commentId)}`, command);
    }

    deleteComment(commentId: string): Observable<Result> {
        return this.http.delete<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommentById(commentId)}`);
    }

    getCommentById(commentId: string): Observable<Result<CommentDetailsDto>> {
        return this.http.get<Result<CommentDetailsDto>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommentById(commentId)}`);
    }

    getPostComments(postId: string, currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<CommentSummaryDto>>> {
        const params = new HttpParams()
            .set('page', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<CommentSummaryDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.PostComments(postId)}`, { params });
    }

    getCommentReplies(commentId: string, currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<CommentSummaryDto>>> {
        const params = new HttpParams()
            .set('page', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<CommentSummaryDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommentReplies(commentId)}`, { params });
    }

    getUserComments(profileId: string, currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<CommentSummaryDto>>> {
        const params = new HttpParams()
            .set('page', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<CommentSummaryDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.UserComments(profileId)}`, { params });
    }

    getMyComments(currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<CommentSummaryDto>>> {
        const params = new HttpParams()
            .set('page', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<CommentSummaryDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.MyComments}`, { params });
    }
}
