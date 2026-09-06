import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { MediaType } from '../../shared/design-system/components/media-player/media-player';

@Injectable({
    providedIn: 'root'
})
export class ProjectMediaService {
    private http = inject(HttpClient);

    addProjectMedia(projectId: string, file: File, caption: string, type: MediaType | string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectMedia(projectId)}`;
        const formData = new FormData();
        formData.append('file', file);
        formData.append('caption', caption);
        formData.append('type', type.toString());

        return this.http.post<Result>(url, formData);
    }

    deleteProjectMedia(projectId: string, mediaId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectMedia(projectId)}/${mediaId}`;
        return this.http.delete<Result>(url);
    }
}
