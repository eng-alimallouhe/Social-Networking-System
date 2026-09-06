import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { TAGS_API_ROUTES } from '../constants/api-routes/tags-api.routes';
import { Result } from '../contracts/result';
import { TagDto } from '../contracts/tag.dto';

@Injectable({
    providedIn: 'root'
})
export class TagsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getTags(search?: string): Observable<Result<TagDto[]>> {
        const query = search && search.trim().length > 0
            ? `?search=${encodeURIComponent(search.trim())}`
            : '';
        return this.http.get<Result<TagDto[]>>(`${this.baseUrl}${TAGS_API_ROUTES.Tags}${query}`);
    }
}
