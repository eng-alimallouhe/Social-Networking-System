import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { AddProjectTagCommand } from '../contracts/project-tag.contracts';
import { TagDto } from '../../shared/contracts/tag.dto';
import { TagsService } from '../../shared/services/tags.service';

@Injectable({
    providedIn: 'root'
})
export class ProjectTagsService {
    private http = inject(HttpClient);
    private tagsService = inject(TagsService);

    addProjectTag(projectId: string, command: AddProjectTagCommand): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectTags(projectId)}`;
        return this.http.post<Result>(url, command);
    }

    removeProjectTag(projectId: string, projectTagId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectTags(projectId)}/${projectTagId}`;
        return this.http.delete<Result>(url);
    }

    getTags(search?: string): Observable<Result<TagDto[]>> {
        return this.tagsService.getTags(search);
    }
}
