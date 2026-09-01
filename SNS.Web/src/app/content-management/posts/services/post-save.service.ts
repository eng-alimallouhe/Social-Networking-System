import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../shared/constants/api-routes/content-management-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { PostOverviewDto } from '../contracts/post-model.dto';

@Injectable({
    providedIn: 'root'
})
export class PostSaveService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    getSavedPosts(currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<PostOverviewDto>>> {
        const params = new HttpParams()
            .set('currentPage', currentPage.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<PostOverviewDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.SavedPosts}`, { params });
    }

    savePost(postId: string): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.SavePost(postId)}`, {});
    }

    unsavePost(postId: string): Observable<Result> {
        return this.http.delete<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.UnsavePost(postId)}`);
    }
}
