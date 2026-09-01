import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../shared/constants/api-routes/content-management-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { PostOverviewDto } from '../contracts/post-model.dto';
import { PostDetailsDto } from '../contracts/post-details.dto';
import { CreatePostCommand } from '../contracts/create-post.command';
import { UpdatePostCommand } from '../contracts/update-post.command';

@Injectable({
    providedIn: 'root'
})
export class PostsService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    createPost(command: CreatePostCommand): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.Posts}`, command);
    }

    updatePost(postId: string, command: UpdatePostCommand): Observable<Result> {
        return this.http.put<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.PostById(postId)}`, command);
    }

    deletePost(postId: string): Observable<Result> {
        return this.http.delete<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.PostById(postId)}`);
    }

    getFeed(currentPage: number = 1, pageSize: number = 10): Observable<Result<PostOverviewDto[]>> {
        const params = new HttpParams()
            .set('currentPage', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<PostOverviewDto[]>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.Feed}`, { params });
    }

    getPostById(postId: string): Observable<Result<PostDetailsDto>> {
        return this.http.get<Result<PostDetailsDto>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.PostById(postId)}`);
    }

    getUserReactedPosts(currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<PostOverviewDto>>> {
        const params = new HttpParams()
            .set('currentPage', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<PostOverviewDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.ReactedPosts}`, { params });
    }

    getUserPosts(profileId: string, currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<PostOverviewDto>>> {
        const params = new HttpParams()
            .set('currentPage', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<PostOverviewDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.UserPosts(profileId)}`, { params });
    }

    increaseInterest(postId: string): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.IncreasePostInterest(postId)}`, {});
    }

    decreaseInterest(postId: string): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.DecreasePostInterest(postId)}`, {});
    }
}
